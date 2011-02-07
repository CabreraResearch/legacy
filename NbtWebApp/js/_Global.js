// ------------------------------------------------------------------------------------
// Enum definitions
// ------------------------------------------------------------------------------------

// For CswImageButton
var CswImageButton_ButtonType = {
    None: -1,
    Add: 27,
    ArrowNorth: 28,
    ArrowEast: 29,
    ArrowSouth: 30,
    ArrowWest: 31,
    Calendar: 6,
    CheckboxFalse: 18,
    CheckboxNull: 19,
    CheckboxTrue: 20,
    Clear: 4,
    Clock: 10,
    ClockGrey: 11,
    Configure: 26,
    Delete: 4,
    Edit: 3,
    Fire: 5,
    PageFirst: 23,
    PagePrevious: 24,
    PageNext: 25,
    PageLast: 22,
    PinActive: 17,
    PinInactive: 15,
    Print: 2,
    Refresh: 9,
    SaveStatus: 13,
    Select: 32,
    ToggleActive: 1,
    ToggleInactive: 0,
    View: 8
}

// ------------------------------------------------------------------------------------
// Cookies
// ------------------------------------------------------------------------------------

function SetSessionId(SessionId) {
    $.cookie('CswSessionId', SessionId);
}
function GetSessionId() {
    return $.cookie('CswSessionId');
}
function ClearSessionId() {
    $.cookie('CswSessionId', null);
}

function SetUsername(Username) {
    $.cookie('csw_username', Username);
}
function GetUsername() {
    return $.cookie('csw_username');
}
function ClearUsername() {
    $.cookie('csw_username', null);
}


// ------------------------------------------------------------------------------------
// Ajax
// ------------------------------------------------------------------------------------


function CswAjaxJSON(options) {
    var o = {
        url: '',
        data: '',
        success: function (result) { }
    };

    if (options) {
        $.extend(o, options);
    }

    var starttime = new Date();
    $.ajax({
        type: 'POST',
        url: o.url,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: o.data,
        success: function (data, textStatus, XMLHttpRequest) {
            var endtime = new Date();
            $('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

            o.success($.parseJSON(data.d));

        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
        }
    });      // $.ajax({
} // CswAjaxXml()

function CswAjaxXml(options) {
    var o = {
        url: '',
        data: '',
        success: function ($xml) { }
    };

    if (options) {
        $.extend(o, options);
    }

    var starttime = new Date();
    $.ajax({
        type: 'POST',
        url: o.url,
        dataType: "xml",
        //contentType: 'application/json; charset=utf-8',
        data: o.data,     // should be 'field1=value&field2=value'
        success: function (data, textStatus, XMLHttpRequest) {

            var endtime = new Date();
            $('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

            // this is IE compliant
            var $xml = $(XMLHttpRequest.responseXML);
            var $realxml = $xml.children().first();
            if ($realxml.first().get(0).nodeName == "error") {
                _handleAjaxError(XMLHttpRequest, $realxml.text(), '');
            }
            else {
                o.success($realxml);
            }

        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
        }
    });            // $.ajax({
} // CswAjaxXml()
        
function _handleAjaxError(XMLHttpRequest, textStatus, errorThrown) 
{
    ErrorMessage = "Error: " + textStatus;
    if (null != errorThrown) {
        ErrorMessage += "; Exception: " + errorThrown.toString()
    }
    console.log(ErrorMessage);
} // _handleAjaxError()

//function extractCDataValue($node) {
//    // default
//    ret = $node.text();

//    // for some reason, CDATA fields come through from the webservice like this:
//    // <node><!--[CDATA[some text]]--></node>
//    var cdataval = $node.html();
//    if (cdataval != undefined && cdataval != '') {
//        var prefix = '<!--[CDATA[';
//        var suffix = ']]-->';

//        if (cdataval.substr(0, prefix.length) == prefix) {
//            ret = cdataval.substr(prefix.length, cdataval.length - prefix.length - suffix.length);
//        }
//    }
//    return ret;
//}

function xmlToString($xmlnode) {
    var xmlstring = $xmlnode.get(0).xml; // IE
    if (!xmlstring) {            // FF, Chrome, Safari
        var s = new XMLSerializer();
        xmlstring = s.serializeToString($xmlnode.get(0));
    }
    if (!xmlstring) {
        $.error("Browser does not support XML operations necessary to convert to string");
    }
    return xmlstring;
}


// ------------------------------------------------------------------------------------
// Popups and Dialogs
// ------------------------------------------------------------------------------------

function OpenPopup(popupurl) {
    var popup = window.open(popupurl, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
    popup.focus();
    return popup;
}

function OpenDialog(id, url) {
    var $dialogdiv = $('<div id="' + id + '" style="display: none;"></div>');
    $dialogdiv.load(url,
                    {},
                    function (responseText, textStatus, XMLHttpRequest) {
                        $dialogdiv.appendTo('body')
                                  .dialog();
                    });
}

function CloseDialog(id) {
    $('#' + id).remove();
}



// ------------------------------------------------------------------------------------
// Layout mechanics
// ------------------------------------------------------------------------------------

function makeTable(id) 
{
    return $('<table id="'+ id +'" cellpadding="0" cellspacing="0" border="0"><tr><td></td></tr></table>');
}

// row and col are 1-based
function getTableCell($table, row, col) {
    var $cell = null;

    if ($table.length > 0 &&
         row != undefined && row != '' &&
         col != undefined && col != '' ) 
    {
        while (row >= $table.children('tbody').children('tr').length) {
            $table.append('<tr></tr>');
        }
        var $row = $($table.children('tbody').children('tr')[row]);
        while (col >= $row.children('td').length) {
            $row.append('<td></td>');
        }
        var $cell = $($row.children('td')[col]);
    }
    return $cell;
}

// ------------------------------------------------------------------------------------
// strings
// ------------------------------------------------------------------------------------

function startsWith(source, search) 
{
    return (source.substr(0, search.length) == search);
}

// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj) {
    var str;
    for (var x in obj) {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    if (popup != null)
        popup.document.write(str);
    else
        console.log("iterate() error: No popup!");
}



