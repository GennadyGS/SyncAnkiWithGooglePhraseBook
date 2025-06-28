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

    if (message.command === "log") {
        if (message.args.level === "info") {
            console.log(message.args.message);
        }
        else if (message.args.level === "error") {
            console.error(message.args.message);
        }
    }

    return true; // important: enables async response});
});


window.logInfo = function (tabId, message) {
    chrome.tabs.sendMessage(
        tabId,
        {
            command: "log",
            args: {
                level: "info",
                message: message
            }
        });
}

function logError(tabId, message) {
    chrome.tabs.sendMessage(
        tabId,
        {
            command: "log",
            args: {
                level: "error",
                message: message
            }
        });
}

console.log("content script loaded");