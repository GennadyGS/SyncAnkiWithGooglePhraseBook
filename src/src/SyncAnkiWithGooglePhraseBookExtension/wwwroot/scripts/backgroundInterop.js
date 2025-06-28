window.waitForTabToLoad = function (tabId) {
    return new Promise((resolve) => {
        function listener(updatedTabId, changeInfo) {
            if (updatedTabId === tabId && changeInfo.status === "complete") {
                console.log("Window is loaded")
                chrome.tabs.onUpdated.removeListener(listener);
                resolve();
            }
        }
        chrome.tabs.get(tabId, function (tab) {
            if (tab.status === "complete") {
                console.log("Window is already loaded")
                resolve();
            } else {
                console.log("Adding tab listener")
                chrome.tabs.onUpdated.addListener(listener);
            }
        });
    });
};
