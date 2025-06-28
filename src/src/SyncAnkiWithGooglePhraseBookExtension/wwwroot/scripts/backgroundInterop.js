window.waitForTabToLoad = function (tabId) {
    return new Promise((resolve) => {
        function listener(updatedTabId, changeInfo) {
            if (updatedTabId === tabId && changeInfo.status === "complete") {
                chrome.tabs.onUpdated.removeListener(listener);
                resolve();
            }
        }
        chrome.tabs.get(tabId, function (tab) {
            if (tab.status === "complete") {
                resolve();
            } else {
                chrome.tabs.onUpdated.addListener(listener);
            }
        });
    });
};