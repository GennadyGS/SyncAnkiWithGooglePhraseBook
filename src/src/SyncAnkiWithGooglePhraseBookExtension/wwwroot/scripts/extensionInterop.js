chrome.runtime.onMessage.addListener(async (message, sender, sendResponse) => {
    if (message.command === "clickButton") {
        try {
            var btn = document.querySelector(message.selector);
            if (btn) btn.click();
            sendResponse({ success: true });
        } catch (e) {
            sendResponse({ success: false, error: e.toString() });
        }
    }

    if (message.action === "waitForTabLoad") {
        const tabId = message.tabId;
        const timeoutMs = message.timeoutMs;
        try {
            await waitForTabLoad(tabId, timeoutMs);
            sendResponse({ success: true });
        } catch (e) {
            sendResponse({ success: false, error: e.message });
        }
    }
    return true; // important: enables async response});
});

(() => {
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

window.extensionLogger = {
    logInfo: function (message) { console.log("INFO:", message); },
    logError: function (message) { console.error("ERROR:", message); }
};

console.log("content script loaded");