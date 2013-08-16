(function () {

    var self = self || this;

    self.addEventListener('message', function (e) {
        
        var data = JSON.stringify(e.data.data);
        var url = e.data.url;
        
        var req = new XMLHttpRequest();

        req.open('POST', '../../' + url, false);

        req.setRequestHeader('Content-Type', 'application/json;  charset=utf-8');

        req.onreadystatechange = function () {
            
            if (req.readyState == 4 && req.status == 200) {
                self.postMessage({ 'error': false, 'data': JSON.parse( req.response ) });
            } else {
                self.postMessage({ 'error': true, 'status': req.statusText, 'data': req.response });
            }
        };
        req.send(data);
    });

    //function toParams(val) {
    //    var ret = '?';
    //    val = val || {};
    //    Object.keys(val).forEach(function(key) {
    //        ret += key + '=' + val[key] + '&';
    //    });
    //    return ret;
    //}

    //self.addEventListener('get', function (e) {

    //    var url = e.url;
    //    var data = toParams(e.data);

    //    var req = new XMLHttpRequest();

    //    req.open('GET', '/' + url + data, false);

    //    req.setRequestHeader('Content-Type', 'application/json;  charset=utf-8');
    //    req.onreadystatechange = function () {
    //        if (req.readyState == 4 && req.status == 200) {
    //            self.postMessage({ 'Error': 'No', 'Message': 'Save Successful' });
    //        }
    //    };
    //    req.send();
    //});

}());