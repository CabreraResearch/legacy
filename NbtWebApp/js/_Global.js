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

//function SetSessionId(SessionId) {
//    $.cookie('CswSessionId', SessionId);
//}
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

function SetCurrentView(options) {
	var o = {
		viewid: '',
		viewmode: ''
	};
	if (options)
	{
		$.extend(o, options);
	}
	$.cookie('csw_currentviewid', o.viewid);
	$.cookie('csw_currentviewmode', o.viewmode);
}

function GetCurrentView()
{
	var view = {
		viewid: $.cookie('csw_currentviewid'),
		viewmode: $.cookie('csw_currentviewmode')
	};
	return view;
}

function ClearCurrentView()
{
	$.cookie('csw_currentviewid', null);
	$.cookie('csw_currentviewmode', null);
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
				_handleAjaxError(XMLHttpRequest, $realxml.text().trim(), '');
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
	ErrorMessage = "A WebServices Error Occurred: " + textStatus;
	if (null != errorThrown) {
		ErrorMessage += "; Exception: " + errorThrown.toString()
	}
	log(ErrorMessage);
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
// Node interactions
// ------------------------------------------------------------------------------------
function copyNode(options)
{
	var o = {
		'nodeid': '',
		'nodekey': '',
		'onSuccess': function (nodeid, nodekey) { }
	};
	if (options)
	{
		$.extend(o, options);
	}
	CswAjaxJSON({
		url: '/NbtWebApp/wsNBT.asmx/CopyNode',
		data: '{ "NodePk":"' + o.nodeid + '" }',
		success: function (result)
		{
			o.onSuccess(result.NewNodeId, '');
		}
	});
}

function deleteNode(options) {
	var o = {
		'nodeid': '',
		'onSuccess': function (nodeid, nodekey) { }
	};
	if (options) {
		$.extend(o, options);
	}

	CswAjaxJSON({
		url: '/NbtWebApp/wsNBT.asmx/DeleteNode',
		data: '{ "NodePk":"' + o.nodeid + '" }',
		success: function (result) {
			o.onSuccess('', '');  // returning '' will reselect the first node in the tree
		}
	});
}


// ------------------------------------------------------------------------------------
// jsTree
// ------------------------------------------------------------------------------------

function jsTreeGetSelected($treediv, IDPrefix) 
{
	$SelectedItem = $treediv.jstree('get_selected');

	var iconurl = $SelectedItem.children('a').children('ins').css('background-image');
	var id = $SelectedItem.attr('id').substring(IDPrefix.length);
	var text = $SelectedItem.children('a').first().text().trim();
	var viewmode = $SelectedItem.attr('viewmode');
	var viewid = $SelectedItem.attr('viewid');
	var type = $SelectedItem.attr('type');
	var nodekey = $SelectedItem.attr('cswnbtnodekey');

	var ret = { 
		'SelectedIconUrl': iconurl,
		'SelectedId': id,
		'SelectedText': text,
		'SelectedViewMode': viewmode,
		'SelectedViewId': viewid,
		'SelectedType': type,
		'SelectedCswNbtNodeKey': nodekey
	};
	return ret;
}


// ------------------------------------------------------------------------------------
// Menu
// ------------------------------------------------------------------------------------

function GoHome() 
{
	ClearCurrentView();
	window.location = "NewMain.html";
}

function HandleMenuItem(options) {
	var o = {
		'$ul': '',
		'$this': '',
		'onLogout': function () { },
		'onAlterNode': function (nodeid, nodekey) { }
	};
	if (options) {
		$.extend(o, options);
	}

	var $li;
	if (o.$this.attr('href') != undefined && o.$this.attr('href') != '')
	{
		$li = $('<li><a href="' + o.$this.attr('href') + '">' + o.$this.attr('text') + '</a></li>')
						.appendTo(o.$ul)
	}
	else if (o.$this.attr('popup') != undefined && o.$this.attr('popup') != '')
	{
		$li = $('<li class="headermenu_dialog">' + o.$this.attr('text') + '</li>')
						.appendTo(o.$ul)
						.click(function () { OpenDialog(o.$this.attr('text'), o.$this.attr('popup')); });
	}
	else if (o.$this.attr('action') != undefined && o.$this.attr('action') != '')
	{
		$li = $('<li><a href="#">' + o.$this.attr('text') + '</a></li>')
						.appendTo(o.$ul);
		var $a = $li.children('a');
		switch (o.$this.attr('action'))
		{

			case 'About':
				$a.click(function () { $.CswDialog('AboutDialog'); return false; });
				break;

			case 'AddNode':
				$a.click(function ()
				{
					$.CswDialog('AddNodeDialog', {
						'nodetypeid': o.$this.attr('nodetypeid'),
						'onAddNode': o.onAlterNode
					}); 
					return false;
				});
				break;

			case 'DeleteNode':
				$a.click(function ()
				{
					$.CswDialog('DeleteNodeDialog', {
						'nodename': o.$this.attr('nodename'),
						'nodeid': o.$this.attr('nodeid'),
						'onDeleteNode': o.onAlterNode
					});
					return false;
				});
				break;

			case 'CopyNode':
				$a.click(function ()
				{
					$.CswDialog('CopyNodeDialog', {
						'nodename': o.$this.attr('nodename'),
						'nodeid': o.$this.attr('nodeid'),
						'onCopyNode': o.onAlterNode
					});
					return false;
				});
				break;

			case 'Home':
				$a.click(function () { GoHome(); return false; });
				break;

			case 'Logout':
				$a.click(function () { o.onLogout(); return false; });
				break;
		}
	}
	else {
		$li = $('<li>' + o.$this.attr('text') + '</li>')
						.appendTo(o.$ul)
	}
	return $li;
}


// ------------------------------------------------------------------------------------
// Validation
// ------------------------------------------------------------------------------------

function validateFloatMinValue(value, minvalue) {
	var nValue = parseFloat(value);
	var nMinValue = parseFloat(minvalue);
	var isValid = true;

	if (nMinValue != undefined)
	{
		if (nValue == undefined || nValue < nMinValue) {
			isValid = false;
		}
	}
	return isValid; 
} // validateFloatMinValue()

function validateFloatMaxValue(value, maxvalue) {
	var nValue = parseFloat(value);
	var nMaxValue = parseFloat(maxvalue);
	var isValid = true;

	if (nMaxValue != undefined) {
		if (nValue == undefined || nValue > nMaxValue) {
			isValid = false;
		}
	}
	return isValid; 
} // validateFloatMaxValue()

function validateFloatPrecision(value, precision) {
	var isValid = true;

	var regex;
	if (precision > 0) {
		// Allow any valid number -- we'll round later
		regex = /^\-?\d*\.?\d*$/g;
	}
	else {
		// Integers Only
		regex = /^\-?\d*$/g;
	}
	if (isValid && !regex.test(value)) {
		isValid = false;
	}

	return isValid;
} // validateFloatPrecision()

function validateInteger(value) {
	// Integers Only
	var regex = /^\-?\d*$/g;
	return (regex.test(value));
} // validateInteger()

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

// because IE 8 doesn't support console.log unless the console is open (*duh*)
function log(s) {
	try { console.log(s) } catch (e) { alert(s) }
};


// ------------------------------------------------------------------------------------
// Browser Compatibility
// ------------------------------------------------------------------------------------

// for IE 8
if (typeof String.prototype.trim !== 'function') {
	String.prototype.trim = function () {
		return this.replace(/^\s+|\s+$/g, '');
	}
}


// Validation Hack
// This is a workaround to a problem introduced by using jquery.validation with jquery 1.5
// http://stackoverflow.com/questions/5068822/ajax-parseerror-on-verrorsfoundtrue-vmessagelogin-failed
// http://blog.m0sa.net/2011/02/jqueryvalidation-breaks-jquery-15-ajax.html

//$(function () {
//	$.ajaxSettings.cache = false;
//	$.ajaxSettings.jsonp = undefined;
//	$.ajaxSettings.jsonpCallback = undefined;
//})
