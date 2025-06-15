chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.command === "clickButton") {
        try {
            var btn = document.querySelector(message.selector);
            if (btn) btn.click();
            sendResponse({ success: true });
        } catch (e) {
            sendResponse({ success: false, error: e.toString() });
        }
    }
});
console.log("content script loaded");