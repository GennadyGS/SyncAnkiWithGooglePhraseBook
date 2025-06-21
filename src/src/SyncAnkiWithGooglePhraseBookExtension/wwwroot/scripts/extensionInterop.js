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
        console.log("document.readyState", document.readyState);
        if (document.readyState === "complete") {
            sendResponse();
        } else {
            window.addEventListener(
                "load",
                () => {
                    console.log("loaded: ", document.readyState);
                    sendResponse();
                },
                { once: true });
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