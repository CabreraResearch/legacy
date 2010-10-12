// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj) {
    var str;
    for (var x in obj) {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    popup.document.write(str);
}


