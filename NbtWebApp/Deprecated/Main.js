// ------------------------------------------------------------------------------------
// NBT Javascript library
// ------------------------------------------------------------------------------------


// ------------------------------------------------------------------------------------
// Main Stuff: View Tree, Nodes Tree, Nodes Grid, PropTable Tabs
// ------------------------------------------------------------------------------------

var wsdelimiter = '^';

function getSessionViewId()
{
	var HiddenField = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_SessionViewId');
	if(HiddenField == null)
		HiddenField = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterCenterContent_SessionViewId');
	return HiddenField.value;
}

function gethiddenchangeviewbutton()
{
	var button = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_HiddenChangeViewButton');
	return button;
}
function gethiddenchangeviewfield()
{
	var button = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_HiddenChangeViewId');
	return button;
}

function getMultiEditCheckbox()
{
	var bcb = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_MultiEditCheck');
	return bcb;
}

function getMainTree()
{
	var tree = $find('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_MainTreeView');
	return tree;
}

function getMainCheckAllLink()
{
	return document.getElementById("ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_CheckAllLink");
}

//function getMainStatusImage()
//{
//    return document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_StatusImage_ib_div');
//}

function getMainSaveButton()
{ 
	return document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterRightContent_proptable_TabOuterTable_savebutton');
}

function getMainSelectedNodeKeyBox()
{
	var selectednodekeybox = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_SelectedNodeKeyBox');
	if(selectednodekeybox == null)
		selectednodekeybox = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterCenterContent_SelectedNodeKeyBox');
	return selectednodekeybox;
}
function getViewTreeComboBox()
{
	var box = $find('ctl00_ctl00_ctl00_FormContent_StandardContent_viewtree_treecombo_combo');
	return box;
}
function getMainAjaxTrigger()
{
	var trigger = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_MainAjaxTrigger');
	if(trigger == null)
		trigger = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterCenterContent_MainAjaxTrigger');
	return trigger;
}
function getMainTreeAjaxTrigger()
{
	var trigger = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_MainNodeTreeAjaxTrigger');
	if(trigger == null)
		trigger = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterCenterContent_MainNodeTreeAjaxTrigger');
	return trigger;
}
function getMainHiddenDeleteButton()
{
	var button = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_HiddenDeleteButton');
	if(button == null)
		button = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterCenterContent_HiddenDeleteButton');
	return button;
}
function getMainHiddenRefreshButton()
{
	var button = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterLeftContent_HiddenRefreshButton');
	if(button == null)
		button = document.getElementById('ctl00_ctl00_ctl00_FormContent_StandardContent_MasterCenterContent_HiddenRefreshButton');
	return button;
}

function getSelectedNodeKey()
{
	var tree = getMainTree();
	var node = tree.get_selectedNode();
	return node.get_value();
}

function getComboIdFromTreeId(treeid)
{
	var idstr = treeid.split('_');
	var comboid = "";
	for(i in idstr)
	{
		if(idstr[i] != "i0")
		{
			if(comboid != "") comboid += "_";
			comboid += idstr[i];
		} else {
			break;
		}
	}
	return comboid;
}


function onSetPropRequired( requiredid, setonaddid, filterid, filterpropselectid ) 
{
	var requiredcheckbox = document.getElementById( requiredid );
	var setonaddcheckbox = document.getElementById( setonaddid );
	var filter = document.getElementById( filterid );
	var filterpropselect = document.getElementById( filterpropselectid );
	
	if( requiredcheckbox.checked == 1 )
	{
	   setonaddcheckbox.checked = 1;
	   setonaddcheckbox.disabled = true;
	   filterpropselect.selectedIndex = 0;
	   filter.disabled = true;
	} else 
	{
//       setonaddcheckbox.checked  = 0;
	   setonaddcheckbox.disabled = false;
	   filter.disabled = false;
	}

	
	return true;
}

function CswViewTree_OnClientNodeClicked(sender, eventArgs)
{
	var node = eventArgs.get_node();
	var nodevalue = node.get_value();

	var comboBox = getViewTreeComboBox();
	comboBox.set_text(node.get_text());
	comboBox.hideDropDown();

	StopPropagation();
}

function BatchOpsMenuItem_Click()
{
	var checkbox = getMultiEditCheckbox();
	checkbox.click();
}


function PropTable_MultiEditCheck_Click()
{
	setChanged();
}



function StopPropagation(e)
{
	 if(!e)
	 {
		e = window.event;
	 }
	 if(e)
		 e.cancelBubble = true;
}


function setCheckAll( treeid , checked )
{
	var tree = $find( treeid );
	UpdateAllTreeNodeChildrenChecked(tree.get_nodes(), checked)
	return false;
}

function UpdateAllTreeNodeChildrenChecked(nodes, checked)
{
   var i;
   for (i=0; i<nodes.get_count(); i++)
   {
	   if (checked)
	   {
		   nodes.getNode(i).check();
	   }
	   else
	   {
		   nodes.getNode(i).set_checked(false);
	   }
	   
	   if (nodes.getNode(i).get_nodes().get_count()> 0)
	   {
		   UpdateAllTreeNodeChildrenChecked(nodes.getNode(i).get_nodes(), checked);
	   }
   }
}


var bSetCheckAllState;
bSetCheckAllState = false;
function Main_SetCheckAll( treeid , linkid )
{
	bSetCheckAllState = !bSetCheckAllState;
	var tree = $find( treeid );
	Main_UpdateAllTreeNodeChildrenChecked(tree, tree.get_nodes(), bSetCheckAllState)    
	
	return false;
}

function Main_UpdateAllTreeNodeChildrenChecked(tree, nodes, checked)
{
   var selectednode = tree.get_selectedNode();
   var selectednodetypeid = getNodeTypeFromTreeNode(selectednode);
	
   var i;
   for (i=0; i<nodes.get_count(); i++)
   {
	   if (checked)
	   {
		   var node = nodes.getNode(i);
		   var thisnodetypeid = getNodeTypeFromTreeNode(node);
		   if(selectednodetypeid == thisnodetypeid)
			   node.check();
	   }
	   else
	   {
		   nodes.getNode(i).set_checked(false);
	   }
	   
	   if (nodes.getNode(i).get_nodes().get_count()> 0)
	   {
		   Main_UpdateAllTreeNodeChildrenChecked(tree, nodes.getNode(i).get_nodes(), checked);
	   }
   }

	var link = getMainCheckAllLink();
	bSetCheckAllState = checked;
	if(link != null)
	{
		if(checked)
			link.innerText = "Uncheck All";
		else
			link.innerText = "Check All";
	}
}

function CheckBoxArray_CheckAllClick(checkalllink, checkboxtable)
{
	var link = document.getElementById(checkalllink);
	var tbl = document.getElementById(checkboxtable);
	var inputs = tbl.getElementsByTagName("input"); 

	for( i = 0; i < inputs.length; i++) 
	{ 
		if(inputs[i].type=="checkbox")
		{
			if(link.innerText == "Check All")
			{
				inputs[i].checked = true;
			} else {
				inputs[i].checked = false;
			}
		}
	}
	if(link.innerText == "Check All")
	{
		link.innerText = "Uncheck All";
	} else {
		link.innerText = "Check All";
	}
	setChanged();
}


function getMainTreeCheckedNodeIds()
{
	var tree = getMainTree();
	if(tree != null)
	{
		var nodes = tree.get_nodes();
		return getMainTreeCheckedNodeIdsRecursive(nodes);
	} else {
		return "";
	}
}

function getMainTreeCheckedNodeIdsRecursive(nodes)
{
	var ret = "";
	for (var i=0; i<nodes.get_count(); i++)
	{
		if(nodes.getNode(i).get_checked())
		{
			if(ret != "") ret += ",";
			ret += getNodeIdFromTreeNode(nodes.getNode(i));
		}
		
		var ret2 = "";
		if(nodes.getNode(i).get_nodes().get_count() > 0)
			ret2 = getMainTreeCheckedNodeIdsRecursive(nodes.getNode(i).get_nodes())
		
		if(ret2 != "")
		{    
			if(ret != "") ret += ",";
			ret += ret2;
		}
	}
	return ret;
}

function getNodeIdFromTreeNode(treenode)
{
	var thisnodekey = treenode.get_value();
	var thisnodekeysplit = thisnodekey.split(':');
	var thisnodeid = thisnodekeysplit[1];
	return thisnodeid;
}
function getNodeTypeFromTreeNode(treenode)
{
	var thisnodekey = treenode.get_value();
	var thisnodekeysplit = thisnodekey.split(':');
	var thisnodetypeid = thisnodekeysplit[3];
	return thisnodetypeid;
}

function MainNodeTree_OnNodeChecking(sender, args)
{
	var node = args.get_node();
	var thisnodetypeid = getNodeTypeFromTreeNode(node);
		
	var tree = node.get_treeView();
	var selectednode = tree.get_selectedNode();
	var selectednodetypeid = getNodeTypeFromTreeNode(selectednode);
	
	if(selectednodetypeid != thisnodetypeid)
		args.set_cancel(true);
}


var pagesize = 20;    //if this is changed, change Main.aspx.cs too
var morekey = "";
var scrollnode;

function MainNodeTree_OnNodeClicking(sender, args)
{
	var node = args.get_node();
	var tree = node.get_treeView();
	var priorselectednode = tree.get_selectedNode();
	if(nodeIsMoreNode(node))
	{
		//fetch more nodes
		scrollnode = node.get_previousNode();
		morekey = node.get_value();
		tree._webServiceSettings.set_path('TreeViewService.asmx');
		tree._webServiceSettings.set_method('GetMoreNodes');
		tree._loadChildrenFromWebService(node.get_parent());

		setTimeout(function() { priorselectednode.select(); }, 1000);
	}
	else
	{
		//If we switch nodetypes, clear checked nodes
		var priornodetypeid = getNodeTypeFromTreeNode(priorselectednode);
		var newnodetypeid = getNodeTypeFromTreeNode(node);
		if(priornodetypeid != newnodetypeid)
			Main_UpdateAllTreeNodeChildrenChecked(tree, tree.get_nodes(), false);

		if(manuallyCheckChanges())
		{
			var selectednodekeybox = getMainSelectedNodeKeyBox();
			selectednodekeybox.value = node.get_value();
			var ajaxtrigger = getMainAjaxTrigger();
			ajaxtrigger.click();
		} else {
			setTimeout(function() { priorselectednode.select(); }, 200);
			return false;
		}
	}
}

function MainNodeTree_OnNodeClicked(sender, args)
{
	var node = args.get_node();
	if(nodeIsMoreNode(node))
	{
		//cleanup
		var tree = node.get_treeView();
		tree._webServiceSettings.set_path('TreeViewService.asmx');
		tree._webServiceSettings.set_method('GetNodeChildren');
		var parentNode = node.get_parent();     
		setTimeout(function() { parentNode.get_nodes().remove(node); }, 1000);  // too early causes error
	}
}

function MainList_OnClick(nodekey) 
{
	var selectednodekeybox = getMainSelectedNodeKeyBox();
	selectednodekeybox.value = nodekey; 
	var ajaxtrigger = getMainAjaxTrigger();
	ajaxtrigger.click();
	return false;
}

function nodeIsMoreNode(node)
{
	return (node.get_text().substring(0,7) == "More...");
}


function MainNodeTree_OnNodePopulating(sender, eventArgs) 
{
	// for fetching more nodes and child nodes
	var node = eventArgs.get_node();
	var context = eventArgs.get_context();
//	context['ViewType'] = getSelectedViewType();
//	context['ViewId'] = getSelectedViewId();
	context['PageSize'] = pagesize;
	context['ParentNodeKey'] = node.get_value();
	context['MoreNodeKey'] = morekey;
	context['SessionViewId'] = getSessionViewId();
	context['SelectedNodeKey'] = getMainSelectedNodeKeyBox().value;
}


function MainNodeTree_OnNodePopulated(sender, eventArgs)
{
//    if(scrollnode != null && scrollnode != "undefined")
//    {
//        setTimeout(function() {
//          var node = scrollnode.get_nextNode();
//          node.select();
//          scrollnode = null; 
//        }, 1500);  // too early prevents scroll
//    }
}


function NodeTypeTree_OnNodePopulating(sender, eventArgs) 
{
	// for fetching child nodes
	var node = eventArgs.get_node();
	var context = eventArgs.get_context();
	context['Parent'] = node.get_value();
	context['Mode'] = "";
}
function NodeTypeTree_OnNodePopulating_InspectionMode(sender, eventArgs) 
{
	// for fetching child nodes
	var node = eventArgs.get_node();
	var context = eventArgs.get_context();
	context['Parent'] = node.get_value();
	context['Mode'] = "i";
}

function MainGrid_RowClick(sender, eventArgs)
{
	var selectednodekeybox = getMainSelectedNodeKeyBox();
	selectednodekeybox.value = eventArgs.getDataKeyValue("NodeKey");
	var ajaxtrigger = getMainAjaxTrigger();
	ajaxtrigger.click();
}


function MainAjaxTrigger_PreClick()
{
	return manuallyCheckChanges();
}

function MainNodeTreeAjaxTrigger_PreClick()
{
	return manuallyCheckChanges();
}

function CswPropertyTable_SaveButton_PreClick()
{
	// can't do unsetChanged() here, because the Validators 
	// might prevent us from submitting after this event is over
	// see MainAjaxManager_OnResponseEnd()
	if (invalidControlCount > 0)
	{
		alert("Error: Invalid data detected.  Please correct invalid data before saving again.");
	} else
	{
		return true;
	}
}

function CswPropertyTable_TabStrip_OnClientTabSelecting(sender, eventArgs)
{
	if(!manuallyCheckChanges())
		eventArgs.set_cancel(true);
}

function MainAjaxManager_OnResponseEnd(sender, eventArgs)
{
	// for when we return from clicking SaveButton
	unsetChanged();
}


function TimeReportImageButton_Click(sender, eventArgs)
{
	document.getElementById("TimeReport").style.display = "";
}




// ------------------------------------------------------------------------------------
// Grid Export/Printing
// ------------------------------------------------------------------------------------

function openPrintGridPopup(querystring) {
	// This should remain as a popup, rather than a RadWindow.
	OpenPopup('PrintGrid.aspx?' + querystring);
	return false;
}

function openBlobPopup(querystring) {
	// This should remain as a popup, rather than a RadWindow.
	OpenPopup('GetBlob.aspx?' + querystring);
	return false;
}
function openExportPopup(querystring)
{
	// This should remain as a popup, rather than a RadWindow.
	OpenPopup('Popup_Export.aspx?' + querystring);
	return false;
}

function OpenPopup(popupurl)
{
	var popup = window.open(popupurl, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
	popup.focus();
	return popup;
}

// ------------------------------------------------------------------------------------
// Control specific
// ------------------------------------------------------------------------------------


function CswHiddenTable_ToggleExpand(tableid, expandedid) 
{ 
	var table = document.getElementById(tableid); 
	var h = document.getElementById(expandedid); 
	if(table.style.display == 'none') 
	{ 
		table.style.display = ''; 
		h.value = 'true'; 
	} else { 
		table.style.display = 'none'; 
		h.value = 'false'; 
	} 
	return false; 
}

function CswFile_editDocument(hiddenclearid, nodeid, propid)
{
	document.getElementById(hiddenclearid).value = '';
	var url = 'Popup_PutBlob.Aspx?mode=image&nodeid=' + nodeid + '&propid=' + propid;
	var oWnd = window.radopen(null, 'PutBlobDialog');
	oWnd.setUrl(url);
	return false;
}

function CswQuickLaunch_openConfigPopup()
{
	var url = 'Popup_ConfigQuickLaunch.Aspx';
	var oWnd = window.radopen(null, 'ConfigQuickLaunchDialog');
	oWnd.setUrl(url);
	return false;
}
function CswQuickLaunch_CallBack(radWindow, returnValue) 
{
}

function CswFile_clearDocument(hiddenclearid, filelinkid)
{
	document.getElementById(hiddenclearid).value = '1';
	document.getElementById(filelinkid).style.display = 'none';  
	setChanged(); 
	return false;
}

function CswImage_editImage(hiddenclearid, nodeid, propid)
{
	document.getElementById(hiddenclearid).value = '';
	var url = 'Popup_PutBlob.Aspx?mode=image&nodeid=' + nodeid + '&propid=' + propid;
	var oWnd = window.radopen(null, 'PutBlobDialog');
	oWnd.setUrl(url);
	return false;
}

function CswImage_clearImage(hiddenclearid, imageid, labelid)
{
	document.getElementById(hiddenclearid).value = '1';
	document.getElementById(imageid).style.display='none';  
	document.getElementById(labelid).style.display='none';  
	setChanged(); 
	return false;
}

//function CswRelationship_clear(valuelabelid, hiddenclearid)
//{
//    document.getElementById(valuelabelid).innerHTML = '';
//    document.getElementById(hiddenclearid).value = '1';
//    setChanged();
//    return false;
//}


//function CswDatePicker_clearDate(datepickerid, hiddenclearid, hiddentodayid, todaylabelid, todayplusdaysid, useCheckChanges)
//{
//    var datepicker = $find(datepickerid);
//    datepicker.clear(); 
//    var calendarbutton = document.getElementById(datepickerid + '_popupButton');
//    var dateinputtext = document.getElementById(datepickerid + '_dateInput_text');
//    var todaybutton = document.getElementById(datepickerid + '_TodayButton');
//    var todayplusdays = document.getElementById(datepickerid + '_todayplusdays');
//    var todaylabel = document.getElementById(todaylabelid);
//    var hiddenclear = document.getElementById(hiddenclearid);
//    var hiddentoday = document.getElementById(hiddentodayid);

//    if(todaylabel != null)
//        todaylabel.style.display = 'none';
//    if(todayplusdays != null)
//        todayplusdays.style.display = 'none';
//    if(dateinputtext != null)
//        dateinputtext.style.display = '';
//    if(calendarbutton != null)
//        calendarbutton.style.display = '';
//    if(todaybutton != null)
//        todaybutton.style.display = '';
//    if(hiddenclear != null)
//        hiddenclear.value = '1';
//    if(hiddentoday != null)
//        hiddentoday.value = '';
//    
//    if(useCheckChanges) setChanged();
//    return false;
//}
function CswDatePicker_setToday(datepickerid, //hiddenclearid, 
								hiddentodayid, todaylabelid, todayplusdaysid, useCheckChanges)
{
	var todaysDate = new Date();
	var datepicker = $find(datepickerid);
	var calendarbutton = document.getElementById(datepickerid + '_popupButton');
	var dateinputtext = document.getElementById(datepickerid + '_dateInput_text');
	var todaybutton = document.getElementById(datepickerid + '_TodayButton');
	var todayplusdays = document.getElementById(datepickerid + '_todayplusdays');
	var todaylabel = document.getElementById(todaylabelid);
	//var hiddenclear = document.getElementById(hiddenclearid);
	var hiddentoday = document.getElementById(hiddentodayid);

	if(datepicker != null)
		datepicker.set_selectedDate( todaysDate );
		
	if(dateinputtext != null)
		dateinputtext.style.display = 'none';
	if(calendarbutton != null)
		calendarbutton.style.display = 'none';
	if(todaybutton != null)
		todaybutton.style.display = 'none';
//    if(hiddenclear != null)
//        hiddenclear.value = '';
	if(hiddentoday != null)
		hiddentoday.value = '1';
	if(todaylabel != null)
		todaylabel.style.display = '';
	if(todayplusdays != null)
		todayplusdays.style.display = '';

	if(useCheckChanges) setChanged();
	return false;
}

function CswDatePicker_OnDateSelected_UseCheckChanges(sender, args)
{
	var datepickerid = sender.get_id();
	var datepicker = $find(datepickerid);
	//var hiddenclear = document.getElementById(datepickerid + '_hiddenclear');
	var hiddentoday = document.getElementById(datepickerid + '_hiddentoday');
	var todaylabel = document.getElementById(datepickerid + '_todaylabel');
	var calendarbutton = document.getElementById(datepickerid + '_popupButton');
	var dateinputtext = document.getElementById(datepickerid + '_dateInput_text');
	var todaybutton = document.getElementById(datepickerid + '_TodayButton');
	var todayplusdays = document.getElementById(datepickerid + '_todayplusdays');

	if(todaylabel != null)
		todaylabel.style.display = 'none';
	if(todayplusdays != null)
		todayplusdays.style.display = 'none';
	if(dateinputtext != null)
		dateinputtext.style.display = '';
	if(calendarbutton != null)
		calendarbutton.style.display = '';
	if(todaybutton != null)
		todaybutton.style.display = '';
//    if(hiddenclear != null)
//        hiddenclear.value = '';
	if(hiddentoday != null)
		hiddentoday.value = '';

	setChanged();
}

function CswDatePicker_OnDateSelected(sender, args)
{
	var datepickerid = sender.get_id();
	var datepicker = $find(datepickerid);
	//var hiddenclear = document.getElementById(datepickerid + '_hiddenclear');
	var hiddentoday = document.getElementById(datepickerid + '_hiddentoday');
	var todaylabel = document.getElementById(datepickerid + '_todaylabel');
	var calendarbutton = document.getElementById(datepickerid + '_popupButton');
	var dateinputtext = document.getElementById(datepickerid + '_dateInput_text');
	var todaybutton = document.getElementById(datepickerid + '_TodayButton');
	var todayplusdays = document.getElementById(datepickerid + '_todayplusdays');

	if(todaylabel != null)
		todaylabel.style.display = 'none';
	if(todayplusdays != null)
		todayplusdays.style.display = 'none';
	if(dateinputtext != null)
		dateinputtext.style.display = '';
	if(calendarbutton != null)
		calendarbutton.style.display = '';
	if(todaybutton != null)
		todaybutton.style.display = '';
//    if(hiddenclear != null)
//        hiddenclear.value = '';
	if(hiddentoday != null)
		hiddentoday.value = '';
}

//function CswTimePicker_clearTime(hiddenclearid, timepickerid)
//{
//    $find(timepickerid).clear(); 
//    document.getElementById(hiddenclearid).value = '1';
//    return false;
//}

//function CswTimePicker_OnDateSelected(sender, args)
//{
//    var timepickerid = sender.get_id();
//    var hiddenclearid = document.getElementById(timepickerid + '_hiddenclear').id;
//    document.getElementById(hiddenclearid).value = '';
//    setChanged();
//}


function PutBlob_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		var trigger = getMainAjaxTrigger();
		trigger.click();
	}
}

function CswImageButton_ToggleButton(imagebuttonid, defaultIsExpanded) 
{
	var img = document.getElementById(imagebuttonid); 
	if(img.expanded == undefined)
	{
		if(defaultIsExpanded)
			img.expanded = '1';
		else
			img.expanded = '0';
	}
	if(img.expanded == '0') 
	{
		img.onmouseover = function() { this.style.backgroundPosition = '-18px -18px'; }
		img.onmouseout = function() { this.style.backgroundPosition = '0px -18px'; }
		img.style.backgroundPosition = '0px -18px';
		img.oldalt = img.alt;
		img.alt = 'Close';
		img.title = 'Close';
		img.expanded = '1';
	} else {
		img.onmouseover = function() { this.style.backgroundPosition = '-18px 0px'; }
		img.onmouseout = function() { this.style.backgroundPosition = '0px 0px'; }
		img.style.backgroundPosition = '0px 0px';
		if (img.oldalt == null || img.oldalt == '')
		{
			img.alt = 'Edit';
			img.title = 'Edit';
		}
		else
		{
			img.alt = img.oldalt;
			img.title = img.oldalt;
		}
		img.expanded = '0';
	} 
	return false; 
}

function CswMTBF_toggleUnit(mtbfvalueid, hours, days, val)
{
	if(val == 'hours') {
		document.getElementById(mtbfvalueid).innerHTML = hours + ' hours';
	} else {
		document.getElementById(mtbfvalueid).innerHTML = days + ' days';
	}
}

//function CswRelationship_clickNode(node, treeviewclientid) 
//{
//    var viewname = node.get_text();
//    var value = node.get_value();
//    var tree = $find(treeviewclientid);
//}


//function CswRelationship_editNode(treeviewclientid)
//{
//    var tree = $find(treeviewclientid);
//    var node = tree.get_selectedNode();
//    var nodekey = node.get_value();
//    
//    if(nodekey != '')
//    {
//        var popup = OpenPopup('EditNode.aspx?nodekey=' + nodekey);
//    }
//    return false;
//}

function CswRelationship_doAjax(ajaxid)
{
	var ajaxpanel = $find(ajaxid);
	ajaxpanel.AjaxRequest();
}


function CswTimeInterval_showSettings(showval, weeklytableid, monthlytableid, yearlytableid)
{
	var weeklytable = document.getElementById(weeklytableid);
	var monthlytable = document.getElementById(monthlytableid);
	var yearlytable = document.getElementById(yearlytableid);
	
	if(showval == 'weekly')
	{
		weeklytable.style.display = ''; 
		monthlytable.style.display = 'none'; 
		yearlytable.style.display = 'none'; 
	}
	if(showval == 'monthly')
	{
		weeklytable.style.display = 'none'; 
		monthlytable.style.display = ''; 
		yearlytable.style.display = 'none'; 
	}
	if(showval == 'yearly')
	{
		weeklytable.style.display = 'none'; 
		monthlytable.style.display = 'none'; 
		yearlytable.style.display = ''; 
	}
}


function CswTreeCombo_clear(comboid)
{
	var comboBox = $find(comboid);
	comboBox.set_text('');
	comboBox.set_value('');
	return false;
}

function CswTreeCombo_TreeNodeSelect(sender, eventArgs)
{
	var node = eventArgs.get_node()
	var comboid = getComboIdFromTreeId(sender.get_id());
	var comboBox = $find(comboid);
	comboBox.set_text(node.get_text());
	comboBox.set_value(node.get_value());
	comboBox.hideDropDown();
}


// CswTriStateCheckBox

function CswTriStateCheckBox_click(pbbuttonid, valueclientid, checkbuttondivid, autopostback, required) 
{
	var valuebox = document.getElementById(valueclientid);
	var offset = 0;
	var value = "Null";
	
	if (valuebox.value == "Null") {
		// Set to True
		value = "True";
		offset = 20;
	} else if (valuebox.value == "False") {
		if (required == "true") {
			// Set to True
			value = "True";
			offset = 20;
		} else {
			// Set to Null
			value = "Null";
			offset = 19;
		}
	} else if (valuebox.value == "True") {
		// Set to False
		value = "False";
		offset = 18;
	}
	valuebox.value = value;
	
	var checkbutton = document.getElementById(checkbuttondivid);
	checkbutton.style.backgroundPosition = "0px " + offset * -18 + "px";
	checkbutton.onmouseover = function() { this.style.backgroundPosition = "-18px " + offset * -18 + "px"; }
	checkbutton.onmouseout = function() { this.style.backgroundPosition = "0px " + offset * -18 + "px"; }
	
	if (autopostback == "true")
		document.getElementById(pbbuttonid).click();
	return false;
}



// ------------------------------------------------------------------------------------
// Check Changes
// ------------------------------------------------------------------------------------

var changed = 0;
var checkChangesEnabled = true;

function setChanged() 
{
	if (checkChangesEnabled) {
		changed = 1;
//        var statusimage = getMainStatusImage();
		var savebutton = getMainSaveButton();
//        if (statusimage != null) {
//            statusimage.style.backgroundPosition = "0px -210px";
//            statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -210px"; }
//            statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -210px"; }
//            statusimage.title = "There are unsaved changes";
//        } 
		if ( savebutton != null ) {
			savebutton.value = "Save Changes";
			savebutton.disabled = false;
		}
	}
}

function unsetChanged() 
{
	if(checkChangesEnabled)
	{
//        var statusimage = getMainStatusImage();
//        if(statusimage != null)
//            statusimage.style.backgroundPosition = "0px -195px";
//        statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -195px"; }
//        statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -195px"; }
//        statusimage.title = "There are no changes";
		var savebutton = getMainSaveButton();
		if ( savebutton != null ) {
			if ( changed != 0 )
				savebutton.value = "Changes Saved";
			savebutton.disabled = true;
		}
		changed = 0;
	}
}

function checkChanges() 
{
	if (checkChangesEnabled && changed == 1) {
		return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
	}
}

function manuallyCheckChanges()
{
	var ret = true;
	if(checkChangesEnabled && changed == 1) {
		ret = confirm('Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page.');

		// this serves several purposes:
		// 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
		// 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
		if(ret) {
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
		window.onbeforeunload = function() { 
									var f = window.onbeforeunload; 
									var ret = f(); 
									if(ret) { 
										return checkChanges(); 
									} else { 
										return false;
									} 
								};
	} else {
		window.onbeforeunload = function() { 
									return checkChanges(); 
								};
	}

	// IE6 has this annoying habit of throwing unspecified errors if we prevent
	// the navigation with onbeforeunload after clicking a button.
	// So we're going to trap this error and prevent it from being shown.
	window.onerror = function(strError,uri,line) {
		if (strError.toLowerCase().indexOf('unspecified error') >= 0) {
			window.event.returnValue = true;
		} else {
			window.event.returnValue = false;
		}
	}
}

if ((window.onload !== null) && (window.onload !== undefined)) {
	window.onload = new Function('initCheckChanges(); var f=' + window.onload + '; return f();');
} else {
	window.onload = function() { initCheckChanges(); };
}



// ------------------------------------------------------------------------------------
// Validation
// ------------------------------------------------------------------------------------

var invalidControlCount = 0;

function CswFieldTypeWebControl_onchange()
{
	setChanged();
}



// CswNumber
function CswNumber_onchange(numberid, invalidimgid, precision, minvalue, maxvalue)
{
	setChanged();
	var textbox = document.getElementById(numberid);
	var regex;
	var msg;
	var invalidMsg;
	var isValid = true;
	
	// PRECISION
	if (precision > 0)
	{
		// Allow any valid number -- we'll round later if Precision > 0.
		regex = /^\-?\d*\.?\d*$/g;
		msg = 'Value must be numeric'; 
	}
	else
	{
		// Integers Only
		regex = /^\-?\d*$/g;
		msg = 'Value must be an integer'; 
	}
	if(isValid && !regex.test(textbox.value))
	{
		isValid = false; 
		invalidMsg = msg;
	}
	
	// MINVALUE
	if(isValid && minvalue != "" && parseFloat(textbox.value) < minvalue)
	{
			isValid = false;
			invalidMsg = 'Value must be greater than or equal to ' + minvalue;
	}
	// MAXVALUE
	if(isValid && maxvalue != "" && parseFloat(textbox.value) > maxvalue)
	{
			isValid = false;
			invalidMsg = 'Value must be less than or equal to ' + maxvalue;
	}
	
	var img = document.getElementById(invalidimgid);
	if(!isValid)
	{       
		if(img.style.display == 'none')
			invalidControlCount++;
		img.style.display = '';
		img.alt = invalidMsg;
		img.title = invalidMsg;
	} else {
		if(img.style.display == '')
			invalidControlCount--;
		img.style.display = 'none';
		img.alt = '';
		img.title = '';
	}
}

// CswPassword
function CswPassword_onchange(passwordid, confirmid, invalidimgid, length, complexity)
{
	setChanged();
	var p = document.getElementById(passwordid); 
	var c = document.getElementById(confirmid); 
			
	var regex;
	var msg;
	var isValid = true;
	
	// Password and Confirm Password match
	if(isValid && p.value != c.value)
	{
		isValid = false;
		msg = 'Passwords do not match!';
	}

	// Password Length
	if(isValid && p.value.length < length)
	{
		isValid = false;
		msg = 'Password must be at least ' + length + ' characters long';
	}
	
	
	// Password Complexity
	if(isValid && (complexity == 1 || complexity == 2))
	{
		if(complexity == 1)
			msg = 'Password must contain at least one letter and one number';    
		else
			msg = 'Password must contain at least one letter, one number, and one symbol';
			
		// at least one alpha 
		regex = /[\w]/g;
		if(!regex.test(p.value))
		{
			isValid = false;
		}        
		// at least one numeric
		regex = /[\d]/g;
		if(!regex.test(p.value))
		{
			isValid = false;
		}        
		if(complexity == 2)
		{
			// at least one symbol
			regex = /[^\s\d\w]/g;
			if(!regex.test(p.value))
			{
				isValid = false;
			}        
		}
	}

	var img = document.getElementById(invalidimgid);
	if(!isValid)
	{       
		if(img.style.display == 'none')
			invalidControlCount++;
		img.style.display = '';
		img.alt = msg;
		img.title = msg;
	} else {
		if(img.style.display == '')
			invalidControlCount--;
		img.style.display = 'none';
		img.alt = '';
		img.title = '';
	}
}


// CswQuantity
function CswQuantity_onchange(quantityid, unitlistid, invalidimgid, precision, minvalue, maxvalue)
{
	setChanged();
	var isValid = true;
	var quantitybox = document.getElementById(quantityid);
	var unitlist = document.getElementById(unitlistid);
	var regex;
	var msg = '';
	
	// Precision
	if (precision > 0)
	{
		// Allow any valid number -- we'll round later if Precision > 0.
		regex = /^\-?\d*\.?\d*$/g;
		msg = 'Value must be numeric';
	}
	else
	{
		// Integers Only
		regex = /^\-?\d*$/g;
		msg = 'Value must be an integer';
	}

	if(!regex.test(quantitybox.value))
	{
		isValid = false; 
	}

	// Minvalue
	if(isValid && minvalue != "" && parseFloat(quantitybox.value) < minvalue)
	{
		isValid = false;
		msg = 'Value must be greater than or equal to ' + minvalue;
	}
	// Maxvalue
	if(isValid && maxvalue != "" && parseFloat(quantitybox.value) > maxvalue)
	{
		isValid = false;
		msg = 'Value must be less than or equal to ' + maxvalue;
	}
	
	var img = document.getElementById(invalidimgid);
	if(!isValid)
	{       
		if(img.style.display == 'none')
			invalidControlCount++;
		img.style.display = '';
		img.alt = msg;
		img.title = msg;
	} else {
		if(img.style.display == '')
			invalidControlCount--;
		img.style.display = 'none';
		img.alt = '';
		img.title = '';
	}
}

//CswQuestion
function CswQuestion_onchange()
{
	setChanged();
}

// CswNodeTypePermissions and CswNodeTypeSelect
function CswCheckBoxArray_onclick()
{
	setChanged();
}

// CswPropertyFilter
//function CswPropertyFilter_AdvancedLink_Click(SubFieldSelectBoxId, FilterModeSelectBoxId, AdvancedLinkValue)
//{
//    if(document.getElementById(AdvancedLinkValue).value == "1")
//    {
//        document.getElementById(SubFieldSelectBoxId).style.display = "none";
//        document.getElementById(FilterModeSelectBoxId).style.display = "none";
//        document.getElementById(AdvancedLinkValue).value = "0";
//    } else {
//        document.getElementById(SubFieldSelectBoxId).style.display = "";
//        document.getElementById(FilterModeSelectBoxId).style.display = "";
//        document.getElementById(AdvancedLinkValue).value = "1";
//    }
//    return false;
//}

function CswViewFilterEditor_AdvancedLink_Click(ShowAdvancedHiddenField)
{
	document.getElementById(ShowAdvancedHiddenField).value = "true";
}
function CswViewFilterEditor_ClearButton_Click(ClearHiddenField)
{
	document.getElementById(ClearHiddenField).value = "true";
}

// ------------------------------------------------------------------------------------
// MainLayout.aspx
// ------------------------------------------------------------------------------------

function RightHeaderMenu_ItemSelected(menu,args)
{
	var item = args.get_item();
	var ret = false;
	switch (item.get_value())
	{
		case "HomeMenuItem":
			// server event
			ret = true;
			break;
		case "AdminMenuItem":
			// do nothing
			ret = false;
			break;
		case "UserListMenuItem":
			// server event
			ret = true;
			break;
		case "StatisticsMenuItem":
			// server event
			ret = true;
			break;
		case "LogMenuItem":
			// server event
			ret = true;
			break;
		case "ConfigVarsMenuItem":
			// server event
			ret = true;
			break;
		case "PrefsMenuItem":
			// server event
			ret = true;
			break;
		case "HelpTopMenuItem":
			// do nothing
			ret = false;
			break;
		case "HelpMenuItem":
			openHelpPopup();
			ret = false;
			break;
		case "AboutMenuItem":
			openAboutPopup();
			ret = false;
			break;
		case "LogoutMenuItem":
			// server event
			ret = true;
			break;
	}
	return ret;
}


// ------------------------------------------------------------------------------------
// Design
// ------------------------------------------------------------------------------------


function addPropToTemplate(selectelm, textboxid, leftdelim, rightdelim)
{
	var propname = selectelm.value;
	var template = document.getElementById(textboxid);
	if(template.value != '')
		template.value = template.value + ' ';
	template.value = template.value + leftdelim + propname + rightdelim;
	selectelm.selectedIndex = 0;
	return false;
}


// ------------------------------------------------------------------------------------
// Popups
// ------------------------------------------------------------------------------------

//This code is used to provide a reference to the radwindow "wrapper"
function GetRadWindow()
{
	var oWindow = null;
	if (window.radWindow) oWindow = window.radWindow;
	else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
	return oWindow;
}

function Popup_OK_Clicked(param)
{
	var oWindow = GetRadWindow();
	if(param != "" && param != undefined )
		oWindow.close(param);
	else
		oWindow.close(true);
}
function Popup_Cancel_Clicked()
{
	var oWindow = GetRadWindow();			
	oWindow.close(false);
}


// Main - Add Node


function openNewNodePopup( NodeTypeId, ParentNodeKey, SessionViewId, // ViewRelationshipUniqueId, 
						   DontChangeView, DontChangeSelectedNode, SourceViewId )
{
	var oWnd = window.radopen(null, "MainAddDialog");
	var CheckedNodeIds = getMainTreeCheckedNodeIds();
	//oWnd.setUrl('Popup_EditNode.aspx?dcv=' + DontChangeView + '&dcsn=' + DontChangeSelectedNode + '&nodetypeid=' + NodeTypeId + '&parentnodekey=' + ParentNodeKey + '&svid=' + SessionViewId + '&vrui=' + ViewRelationshipUniqueId + '&checkednodeids=' + CheckedNodeIds);
	oWnd.setUrl('Popup_EditNode.aspx?dcv=' + DontChangeView + '&dcsn=' + DontChangeSelectedNode + '&nodetypeid=' + NodeTypeId + '&parentnodekey=' + ParentNodeKey + '&svid=' + SessionViewId + '&checkednodeids=' + CheckedNodeIds + '&sourceviewid=' + SourceViewId);
}

function MainAdd_CallBack(radWindow, returnValue) 
{
	if(returnValue && !changed)
	{
		var keybox = getMainSelectedNodeKeyBox();
		if(keybox != null)
			keybox.value = "";
		var trigger = getMainTreeAjaxTrigger();
		if(trigger != null)
			trigger.click();
	}
}

function RelationshipAddNodeDialog_openPopup(NodeTypeId)
{
	var oWnd = window.radopen(null, "RelationshipAddNodeDialog");
	oWnd.setUrl('Popup_EditNode.aspx?dcv=1&dcsn=0&nodetypeid=' + NodeTypeId + '&parentnodekey=&svid=&checkednodeids=');
	return false;
}
function RelationshipAddNodeDialog_Callback(radWindow, returnValue) 
{
	if (returnValue && !changed) 
	{
		//var button = getMainAjaxTrigger();
		var button = getMainTreeAjaxTrigger();
		button.click();
	}
}

// Main - Delete Node

function openDeleteNodePopup( NodeKey )
{
	var oWnd = window.radopen(null, "MainDeleteDialog");
	var CheckedNodeIds = getMainTreeCheckedNodeIds();
	oWnd.setUrl('Popup_DeleteNode.aspx?nodekey=' + NodeKey + '&checkednodeids=' + CheckedNodeIds);
}

function MainDelete_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		var keybox = getMainSelectedNodeKeyBox();
		if(keybox != null)
			keybox.value = "";
		var trigger = getMainTreeAjaxTrigger();
		if(trigger != null)
			trigger.click();
	}
}

// Grid Prop - Edit Node

function openEditNodePopup(NodeKey)
{
	var oWnd = window.radopen(null, "MainEditDialog");
	oWnd.setUrl('Popup_EditNode.aspx?nodekey=' + NodeKey);
}

function openEditNodePopupFromNodeId(NodeId)
{
	var oWnd = window.radopen(null, "MainEditDialog");
	oWnd.setUrl('Popup_EditNode.aspx?nodeid=' + NodeId);
}


function MainEdit_CallBack(radWindow, returnValue)
{
	if (!changed)
	{
		var trigger = getMainTreeAjaxTrigger();
		if (trigger != null)
			trigger.click();
	}
}

function MainEditDialog_OnClientClose(sender, eventArgs)
{
	if (!changed)
	{
		var trigger = getMainTreeAjaxTrigger();
		if (trigger != null)
			trigger.click();
	}
}

// Design - Delete

function openDesignDeletePopup(type, value)
{
	var oWnd = window.radopen(null, "DesignDeleteDialog");
	oWnd.setUrl("Popup_DesignDelete.aspx?type=" + type +"&value=" + value);
}

function DesignDelete_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		var button = getMainHiddenDeleteButton();
		button.click();
	}
}

// Design - Add

function openDesignAddPopup( toadd, type, value, mode )
{
	var oWnd = window.radopen(null, 'DesignAddDialog');
	oWnd.setUrl('Popup_DesignAdd.aspx?add='+ toadd +'&type=' + type + '&value=' + value + '&mode=' + mode);
	return false;
}

function DesignAdd_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		var trigger = getMainTreeAjaxTrigger();
		if (trigger == null)
			trigger = getMainHiddenRefreshButton();
		if(trigger != null)
			trigger.click();
	}
}


// About

function openAboutPopup()
{
	var oWnd = window.radopen(null, 'AboutDialog');
	oWnd.setUrl('Popup_About.aspx');
	return false;
}

// Help

function openHelpPopup()
{
//    var oWnd = window.radopen(null, 'HelpDialog');
//    oWnd.setUrl('help/index.htm');
//    return false;
	OpenPopup('help/index.htm');
	return false;
}


// View - Change
function openChangeViewPopup()
{
	var oWnd = window.radopen(null, 'ChangeViewDialog');
	oWnd.setUrl('Popup_ChangeView.aspx');
	return false;
}

function ChangeView_CallBack(radWindow, returnValue)
{
	if (returnValue)
	{
		var button = gethiddenchangeviewbutton();
		var field = gethiddenchangeviewfield();
		field.value = returnValue;
		button.click();
	}
}

// View - Delete

var realdeletebuttonid;
function openDeleteViewPopup(viewid, realdeleteid)
{
	realdeletebuttonid = realdeleteid;
	var oWnd = window.radopen(null, 'DeleteViewDialog');
	oWnd.setUrl('Popup_DeleteView.aspx?viewid=' + viewid);
	return false;
}

function DeleteView_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		document.getElementById(realdeletebuttonid).click();
	}
}

// Edit Property

function EditPropertyInPopup(nodepk, propid)
{
	var oWnd = window.radopen(null, 'EditPropDialog');
	var CheckedNodeIds = getMainTreeCheckedNodeIds();
	oWnd.setUrl('Popup_EditProp.aspx?nodepk=' + nodepk + '&propid=' + propid + '&checkednodeids=' + CheckedNodeIds);
	return false;
}

function EditProp_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		var trigger = getMainTreeAjaxTrigger();
		if(trigger == null)
			trigger = getMainHiddenRefreshButton();
		if(trigger != null)
			trigger.click();
	}
}

function Popup_PrintLabel_Print(EplBoxClientId)
{
	var epl = document.getElementById(EplBoxClientId).value;
	document.getElementById('labelx').EPLScript = epl;
	document.getElementById('labelx').Print();
	return false;
}


// New View

function openNewViewPopup(viewid)
{
	var oWnd = window.radopen(null, 'NewViewDialog');
	oWnd.setUrl('Popup_NewView.aspx?viewid=' + viewid);
	return false;
}

function NewView_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		window.location = "EditView.aspx" + returnValue;
	}
	return false;
}

// Main - Save View As

function openSaveViewAsPopup(sessionviewid)
{
	var oWnd = window.radopen(null, 'SaveViewAsDialog');
	oWnd.setUrl('Popup_NewView.aspx?sessionviewid=' + sessionviewid);
	return false;
}

function SaveViewAs_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		window.location = "Main.aspx" + returnValue;
	}
	return false;
}

function ViewEditor_ModeDropDown_OnChange(ModeDropDownId, GridWidthLabelId, GridWidthBoxId )
{
	var dd = document.getElementById(ModeDropDownId);
	var displayval = 'none';
	if(dd.value == "Grid")
		displayval = '';
	
	document.getElementById(GridWidthLabelId).style.display = displayval;
	document.getElementById(GridWidthBoxId).style.display = displayval;
//    document.getElementById(GridEditModeLabelId).style.display = displayval;
//    document.getElementById(GridEditModeDropDownId).style.display = displayval;
}

function ViewTree_ClearButton_Click(ViewNodeString, HiddenNodeToRemoveField, HiddenRemoveButtonId)
{
	document.getElementById(HiddenNodeToRemoveField).value = ViewNodeString;
	document.getElementById(HiddenRemoveButtonId).click();
	StopPropagation();
}

function CswViewWizard_Previous(WizardId, MinimumPageNo, MultiPageId, PageCounterId, PreviousButtonId, NextButtonId, FinishButtonId, LinkLabelCssClass, SelectedLinkLabelCssClass)
{
	var multiPage = $find(MultiPageId);
	var currentPage = multiPage.get_selectedIndex();
	if(currentPage > 0) 
	{
		var go = true;
		if(currentPage == 1)
		{
			go = false;
			if(confirm("You will lose any changes made to the current view if you continue.  Are you sure?"))
				go = true;
		}
		if(go)
		{
			return CswWizard_Previous(WizardId, MinimumPageNo, MultiPageId, PageCounterId, PreviousButtonId, NextButtonId, LinkLabelCssClass, SelectedLinkLabelCssClass);
		}
	}

	return false; 
} 

function CswWizard_Previous(WizardId, MinimumPageNo, MultiPageId, PageCounterId, PreviousButtonId, NextButtonId, FinishButtonId, LinkLabelCssClass, SelectedLinkLabelCssClass)
{
	var multiPage = $find(MultiPageId);
	var currentPage = multiPage.get_selectedIndex();
	if(currentPage > 0) 
	{
		document.getElementById(WizardId + '_label' + (currentPage+1)).className = LinkLabelCssClass;

		currentPage--;
		multiPage.get_pageViews().getPageView(currentPage).set_selected(true);

		document.getElementById(PageCounterId).value =  document.getElementById(PageCounterId).value - 1;
		//document.getElementById(NextButtonId).value = 'Next';
		document.getElementById(NextButtonId).style.visibility = '';
		if (document.getElementById(PageCounterId).value <= MinimumPageNo)
			document.getElementById(PreviousButtonId).style.display = 'none'; 
		document.getElementById(WizardId + '_label' + (currentPage+1)).className = SelectedLinkLabelCssClass;
	}

	return false; 
} 


// Print Label
function openPrintLabelPopup(nodeid, propid)
{
	var oWnd = window.radopen(null, 'PrintLabelDialog');
	var CheckedNodeIds = getMainTreeCheckedNodeIds();
	oWnd.setUrl('Popup_PrintLabel.aspx?nodeid=' + nodeid + '&propid=' + propid + '&checkednodeids=' + CheckedNodeIds);
	return false;
}

function PrintLabel_CallBack(radWindow, returnValue)
{
}

// Copy Node
function openCopyPopup(nodekey)
{
	var oWnd = window.radopen(null, 'CopyNodeDialog');
	oWnd.setUrl('Popup_CopyNode.aspx?nodekey=' + nodekey);
	return false;
}
function CopyNode_CallBack(radWindow, returnValue)
{
	if(returnValue && !changed)
	{
		var trigger = getMainTreeAjaxTrigger();
		if(trigger != null)
			trigger.click();
	}
}

// Statistics
function OpenStatisticsPopup(accessid, userid, show, startdate, enddate)
{
	var oWnd = window.radopen(null, 'StatisticsDialog');
	oWnd.setUrl('Popup_Statistics.aspx?accessid='+ accessid + '&userid=' + userid + '&show=' + show + '&startdate=' + startdate + '&enddate=' + enddate);
	return false;
}
function Statistics_CallBack(radWindow, returnValue)
{

}


function Popup_NewView_setViewVisibility(NewViewVisibilityDropDownId, NewViewVisibilityRoleDropDownId, NewViewVisibilityUserDropDownId)
{
	var visdd = document.getElementById(NewViewVisibilityDropDownId);
	var roledd = document.getElementById(NewViewVisibilityRoleDropDownId);
	var userdd = document.getElementById(NewViewVisibilityUserDropDownId);

	if(visdd.value == 'Role')
	{
		roledd.style.display = '';
	} else {
		roledd.style.display = 'none';
	}

	if(visdd.value == 'User')
	{
		userdd.style.display = '';
	} else {
		userdd.style.display = 'none';
	}
	return false;
}


function AssignInspection_ScheduleRadio_Click(subtableid)
{
	var table = document.getElementById(subtableid);
	table.style.display = '';
}
function AssignInspection_OnceRadio_Click(subtableid)
{
	var table = document.getElementById(subtableid);
	table.style.display = 'none';
}

function Spread_Update_Click(updatebuttonid)
{
	var updatebutton = document.getElementById(updatebuttonid);
	updatebutton.click();
	return true;
}

function CreateInspection_ShowAll_Click(checkbox, filtereddropdownid, alldropdownid)
{
	var filtereddropdown = document.getElementById(filtereddropdownid);
	var alldropdown = document.getElementById(alldropdownid);
	if(checkbox.checked)
	{
		filtereddropdown.style.display = "none";
		alldropdown.style.display = "";
	} else {
		filtereddropdown.style.display = "";
		alldropdown.style.display = "none";
	}
}


function confirmDemoDataRemoval(event)
{
    var c = confirm('Alert! This function is for the use of ChemSW, Inc. Support and QA ONLY! This will delete ALL demo data. This action is irreversible. Are You Sure?');
    if (!c) {
        event.cancelBubble = true;
        if(event.stopPropagation)
            event.stopPropagation();
        return false;
    } else {
        return true;
    }
}

// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj)
{
	var str;
	for(var x in obj)
	{
		str = str + x + "=" + obj[x] + "<br><br>";
	}
	var popup = window.open("","popup");
	popup.document.write(str);
}
