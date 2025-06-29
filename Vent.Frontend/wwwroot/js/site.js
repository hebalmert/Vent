window.downloadFile = function (fileName, fileType, base64Data) {
    var link = document.createElement("a");
    link.href = "data:" + fileType + ";base64," + base64Data;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};