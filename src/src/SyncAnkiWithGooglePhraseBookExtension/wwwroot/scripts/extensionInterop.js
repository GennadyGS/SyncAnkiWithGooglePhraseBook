window.clickButtonByClass = function (cssClass) {
    var btn = document.querySelector(cssClass);
    if (btn) btn.click();
};
