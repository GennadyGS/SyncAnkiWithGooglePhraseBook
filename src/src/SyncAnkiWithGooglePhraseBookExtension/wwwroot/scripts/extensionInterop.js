chrome.runtime.onMessage.addListener(async (message, sender, sendResponse) => {
    if (message.command === "clickButton") {
        try {
            console.log("clickButton command received with selector:", message.args.selector);
            var btn = document.querySelector(message.args.selector);
            if (btn) btn.click();
            sendResponse({ success: true });
        } catch (e) {
            sendResponse({ success: false, error: e.toString() });
        }
    }

    if (message.command === "waitForTabLoad") {
        (() => {
            console.log("waitForTabLoad command received with args:", message.args);
            const waiters = new Map();

            async function waitForTabLoad(tabId, timeoutMs) {
                // First check the current tab status
                try {
                    const tab = await chrome.tabs.get(tabId);
                    if (tab.status === 'complete') {
                        return;
                    }
                } catch (err) {
                    throw new Error(`Cannot get tab ${tabId}: ${err.message}`);
                }

                // If already waiting for this tab, prevent duplicate registration
                if (waiters.has(tabId)) {
                    return waiters.get(tabId).promise;
                }

                let timeoutId;
                let resolver;
                let rejecter;

                const promise = new Promise((resolve, reject) => {
                    resolver = resolve;
                    rejecter = reject;

                    timeoutId = setTimeout(() => {
                        waiters.delete(tabId);
                        reject(new Error(`Timeout waiting for tab ${tabId} load after ${timeoutMs} ms`));
                    }, timeoutMs);
                });

                waiters.set(tabId, { resolver, rejecter, timeoutId, promise });

                return promise;
            }

            function onTabUpdated(tabId, changeInfo) {
                if (changeInfo.status === 'complete') {
                    const waiter = waiters.get(tabId);
                    if (waiter) {
                        clearTimeout(waiter.timeoutId);
                        waiter.resolver();
                        waiters.delete(tabId);
                    }
                }
            }

            function onTabRemoved(tabId) {
                const waiter = waiters.get(tabId);
                if (waiter) {
                    clearTimeout(waiter.timeoutId);
                    waiter.rejecter(new Error(`Tab ${tabId} was closed before it finished loading.`));
                    waiters.delete(tabId);
                }
            }

            // One-time event listener registration
            if (!window.__tabLoadListenerRegistered) {
                chrome.tabs.onUpdated.addListener(onTabUpdated);
                chrome.tabs.onRemoved.addListener(onTabRemoved);
                window.__tabLoadListenerRegistered = true;
            }

            // Expose globally for Blazor interop
            window.waitForTabLoad = waitForTabLoad;
        })();

        try {
            await waitForTabLoad(message.args.tabId, message.args.timeoutMs);
            sendResponse({ success: true });
        } catch (e) {
            sendResponse({ success: false, error: e.message });
        }
    }

    if (message.command === "logInfo") {
        console.log(message.args.message);
    }

    if (message.command === "logError") {
        console.error(message.args.message);
    }

    return true; // important: enables async response});
});

console.log("content script loaded");