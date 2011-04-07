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
	
	//var starttime = new Date();
	$.ajax({
		type: 'POST',
		url: o.url,
		dataType: "json",
		contentType: 'application/json; charset=utf-8',
		data: o.data,
		success: function (data, textStatus, XMLHttpRequest)
		{
			//var endtime = new Date();
			//$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

			var result = $.parseJSON(data.d);

			if (result.error != undefined)
			{
				_handleAjaxError(XMLHttpRequest, result.error, '');
			}
			else
			{
				o.success(result);
			}
		}, // success{}
		error: _handleAjaxError
	}); // $.ajax({
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
	if (o.url != '')
	{
		//var starttime = new Date();
		$.ajax({
			type: 'POST',
			url: o.url,
			dataType: "xml",
			//contentType: 'application/json; charset=utf-8',
			data: o.data,     // should be 'field1=value&field2=value'
			success: function (data, textStatus, XMLHttpRequest)
			{
				//var endtime = new Date();
				//$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

				// this is IE compliant
				var $xml = $(XMLHttpRequest.responseXML);
				var $realxml = $xml.children().first();
				if ($realxml.first().get(0).nodeName == "error")
				{
					_handleAjaxError(XMLHttpRequest, $realxml.text().trim(), '');
				}
				else
				{
					o.success($realxml);
				}

			}, // success{}
			error: _handleAjaxError
		}); // $.ajax({
	} // if(o.url != '')
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

function jsonToString(j)
{
	if(typeof j == "object")
	{
	 	var ret = '{';
	 	var first = true;
		for (var property in j)
		{
			if (j.hasOwnProperty(property))
			{
				if (!first)
					ret += ',';
				ret += ' \'' + property + '\': ';
				ret += jsonToString(j[property]);
				first = false;
			}
		}
		ret += '}';
	} 
	else
	{
		ret = '\'' + j + '\'';
	}
	return ret;
} // jsonToString

// ------------------------------------------------------------------------------------
// Check Changes
// ------------------------------------------------------------------------------------

var changed = 0;
var checkChangesEnabled = true;

function setChanged()
{
	if (checkChangesEnabled)
	{
		changed = 1;
		//        var statusimage = getMainStatusImage();
		//var savebutton = $('#SaveTab');
		//        if (statusimage != null) {
		//            statusimage.style.backgroundPosition = "0px -210px";
		//            statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -210px"; }
		//            statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -210px"; }
		//            statusimage.title = "There are unsaved changes";
		//        } 
//		if (savebutton != null)
//		{
//			savebutton.value = "Save Changes";
//			savebutton.disabled = false;
//		}
	}
}

function unsetChanged()
{
	if (checkChangesEnabled)
	{
		//        var statusimage = getMainStatusImage();
		//        if(statusimage != null)
		//            statusimage.style.backgroundPosition = "0px -195px";
		//        statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -195px"; }
		//        statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -195px"; }
		//        statusimage.title = "There are no changes";
//		var savebutton = $('#SaveTab');
//		if (savebutton != null)
//		{
//			if (changed != 0)
//				savebutton.value = "Changes Saved";
//			savebutton.disabled = true;
//		}
		changed = 0;
	}
}

function checkChanges()
{
	if (checkChangesEnabled && changed == 1)
	{
		return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
	}
}

function manuallyCheckChanges()
{
	var ret = true;
	if (checkChangesEnabled && changed == 1)
	{
		ret = confirm('Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page.');

		// this serves several purposes:
		// 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
		// 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
		if (ret)
		{
			changed = 0;
		}
	}
	return ret;
}

function initCheckChanges()
{
	// Assign the checkchanges event to happen onbeforeunload
	if ((window.onbeforeunload !== null) && (window.onbeforeunload !== undefined))
	{
		window.onbeforeunload = function ()
		{
			var f = window.onbeforeunload;
			var ret = f();
			if (ret)
			{
				return checkChanges();
			} else
			{
				return false;
			}
		};
	} else
	{
		window.onbeforeunload = function ()
		{
			return checkChanges();
		};
	}

//	// IE6 has this annoying habit of throwing unspecified errors if we prevent
//	// the navigation with onbeforeunload after clicking a button.
//	// So we're going to trap this error and prevent it from being shown.
//	window.onerror = function (strError, uri, line)
//	{
//		if (strError.toLowerCase().indexOf('unspecified error') >= 0)
//		{
//			window.event.returnValue = true;
//		} else
//		{
//			window.event.returnValue = false;
//		}
//	}
}

if ((window.onload !== null) && (window.onload !== undefined))
{
	window.onload = new Function('initCheckChanges(); var f=' + window.onload + '; return f();');
} else
{
	window.onload = function () { initCheckChanges(); };
}



// ------------------------------------------------------------------------------------
// User permissions
// ------------------------------------------------------------------------------------

function IsAdministrator(options)
{
	var o = { 
		'Yes': function() { }, 
		'No': function() { }
	};
	if (options)
	{
		$.extend(o, options);
	}

	CswAjaxJSON({
		url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
		success: function (data)
		{
			if (data.Administrator == "true")
			{
				o.Yes();
			} else
			{
				o.No();
			}
		}
	});
} // IsAdministrator()

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

function deleteNodes(options) {
	var o = {
		'nodeids': [],
		'onSuccess': function (nodeid, nodekey) { }
	};
	if (options) {
		$.extend(o, options);
	}

	var datastr = '{ "NodePks": [';
	var first = true;
	for (var n in o.nodeids)
	{
		if (!first) datastr += ',';
		datastr += '"' + o.nodeids[n] + '"';
		first = false;
	}
	datastr += '] }';

	CswAjaxJSON({
		url: '/NbtWebApp/wsNBT.asmx/DeleteNodes',
		data: datastr,
		success: function (result) {
			o.onSuccess('', '');  // returning '' will reselect the first node in the tree
		}
	});
}


// ------------------------------------------------------------------------------------
// jsTree
// ------------------------------------------------------------------------------------

function jsTreeGetSelected($treediv)
{
	var IDPrefix = $treediv.attr('id');
	$SelectedItem = $treediv.jstree('get_selected');
	var ret = { 
		'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
		'id': $SelectedItem.attr('id').substring(IDPrefix.length),
		'text': $SelectedItem.children('a').first().text().trim(),
		'$item': $SelectedItem
	};
	return ret;
}


// ------------------------------------------------------------------------------------
// Menu
// ------------------------------------------------------------------------------------

function GoHome() 
{
	$.CswCookie('clear', CswCookieName.CurrentView.ViewId);
	$.CswCookie('clear', CswCookieName.CurrentView.ViewMode);
	window.location = "NewMain.html";
}

function HandleMenuItem(options) {
	var o = {
		'$ul': '',
		'$this': '',
		'onLogout': function () { },
		'onAlterNode': function (nodeid, nodekey) { },
		'onSearch': function (treexml, viewid, nodetypeid) { },
		'onMultiEdit': function () { },
		'Multi': false,
		'NodeCheckTreeId': ''
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
						'relatednodeid': o.$this.attr('relatednodeid'), //for Grid Props
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
						'onDeleteNode': o.onAlterNode,
						'NodeCheckTreeId': o.NodeCheckTreeId,
						'Multi': o.Multi
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

            case 'Search':
                $a.click(function ()
                {
                    $.CswDialog('SearchDialog', {
                        'viewid': o.$this.attr('viewid'),
                        'nodetypeid': o.$this.attr('nodetypeid'),
                        'onSearch': o.onSearch
                    });
                    
                    return false;
                });
				break;
			case 'multiedit':
				$a.click(o.onMultiEdit);
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

function validateTime(value)
{
	var isValid = true;
	var regex = /^(\d?\d):(\d\d)\s?([APap][Mm])?$/g;
	var match = regex.exec(value);
	if (match == null)
	{
		isValid = false;
	}
	else
	{
		var hour = parseInt(match[1]);
		var minute = parseInt(match[2]);
		if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
		{
			isValid = false;
		}
	}
	return isValid;
} // validateTime()

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

function getTimeString(date)
{
	var ret = '';
	var hours = date.getHours()
	var minutes = date.getMinutes()
	if (minutes < 10)
	{
		minutes = "0" + minutes
	}
	ret = (hours % 12) + ":" + minutes + " ";
	if (hours > 11)
	{
		ret += "PM";
	} else
	{
		ret += "AM";
	}
	return ret;
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
