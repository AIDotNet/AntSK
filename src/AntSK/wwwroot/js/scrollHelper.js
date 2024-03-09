// scrollHelper.js
function scrollToBottom(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}