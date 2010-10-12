//
//	Copyright?2005. FarPoint Technologies.	All rights reserved.
//

eval("var FarPoint={};");
FarPoint.System={};
FarPoint.System.CheckBrowserByName=function (browsername,version){
var e3=window.navigator.userAgent;
var e4=false;
var e5=(""+browsername).toLowerCase();
if ((e5.indexOf("ms")>=0)||(e5.indexOf("msie")>=0)||(e5.indexOf("ie")>=0))
e4=(e3.indexOf("MSIE")>=1);
else if ((e5.indexOf("safari")>=0)||(e5.indexOf("apple")>=0))
e4=(e3.indexOf("Safari")>=1);
else if ((e5.indexOf("ff")>=0)||(e5.indexOf("firefox")>=0))
e4=(e3.indexOf("Firefox")>=1);
return e4;
};
FarPoint.System.IsChild=function (parent,child){
if (child==null||parent==null)return false;
var e6=child.parentNode;
while (e6!=null){
if (e6==parent)return true;
e6=e6.parentNode;
}
return false;
};
FarPoint.System.FindElementById=function (ctl,id,ATTRI_ID){
if (ctl==null)return null;
var e7=ctl.getAttribute(ATTRI_ID);
if (e7==null)return null;
return document.getElementById(e7+id);
};
FarPoint.System.GetEvent=function (e){
if (e!=null)return e;
return window.event;
};
FarPoint.System.GetTarget=function (e){
e=FarPoint.System.GetEvent(e);
if (e.target==document&&e.currentTarget!=null)return e.currentTarget;
if (e.target!=null)return e.target;
return e.srcElement;
};
FarPoint.System.CancelDefault=function (e){
if (e.preventDefault!=null){
e.preventDefault();
e.stopPropagation();
}else {
e.cancelBubble=true;
e.returnValue=false;
}
return false;
};
FarPoint.System.GetMouseCoords=function (ev){
if (ev.pageX||ev.pageY){
return {x:ev.pageX,y:ev.pageY};
}
return {
x:ev.clientX+document.body.scrollLeft-document.body.clientLeft,
y:ev.clientY+document.body.scrollTop-document.body.clientTop
};
};
FarPoint.System.GetOffsetTop=function (ctl){
var e8=0;
while (ctl){
if ((ctl.tagName!="HTML")&&(typeof(ctl.tagName)!="undefined"))
e8+=typeof(ctl.offsetTop)!="undefined"?ctl.offsetTop:0-typeof(ctl.scrollTop)!="undefined"?ctl.scrollTop:0;
if (typeof(ctl.clientTop)=="number")e8+=ctl.clientTop;
ctl=ctl.offsetParent;
}
return parseInt(e8);
};
FarPoint.System.GetOffsetLeft=function (ctl){
var e9=0;
while (ctl){
if ((ctl.tagName!="HTML")&&(typeof(ctl.tagName)!="undefined"))
e9+=typeof(ctl.offsetLeft)!="undefined"?ctl.offsetLeft:0-typeof(ctl.scrollLeft)!="undefined"?ctl.scrollLeft:0;
if (typeof(ctl.clientLeft)=="number")e9+=ctl.clientLeft;
ctl=ctl.offsetParent;
}
return parseInt(e9);
};
FarPoint.System.AttachEvent=function (target,event,handler,useCapture){
if (target==null||event==null||handler==null)return ;
if (target.addEventListener!=null){
target.addEventListener(event,handler,useCapture);
}else if (target.attachEvent!=null){
target.attachEvent("on"+event,handler);
}
};
FarPoint.System.DetachEvent=function (target,event,handler,useCapture){
if (target==null||event==null||handler==null)return ;
if (target.removeEventListener!=null){
target.removeEventListener(event,handler,useCapture);
}else if (target.detachEvent!=null){
target.detachEvent("on"+event,handler);
}
};
FarPoint.System.Track=function (msg){
if (!FarPoint.System.Config.Consts.$FLAG_ISDEBUG)return ;
if (document.getElementById("txtOutput")==null){
var f0=document.createElement("textarea");
f0.id="txtOutput";
f0.style.width="100%";
f0.style.height="100px";
if (f0.style.bottom!=null&&f0.style.right!=null){
f0.style.bottom="0px";
f0.style.right="0px";
}
f0.style.color="#00ff00";
f0.style.position="absolute";
f0.style.backgroundColor="black";
if (FarPoint.System.CheckBrowserByName("IE")){
window.onload=function (){
if (document.all&&document.body.readyState=="complete"){
document.body.appendChild(f0);
}
};
}else {
document.body.appendChild(f0);
}
}
var f1=document.getElementById("txtOutput");
if (f1!=null){
f1.value="&nbsp;&nbsp;"+msg+"\r\n"+f1.value;
}
};
FarPoint.System.Config={};
FarPoint.System.Config.Consts={};
var f2=FarPoint.System.Config.Consts;
f2.$FLAG_ISDEBUG=false;
f2.$LEFT=37;
f2.$RIGHT=39;
f2.$UP=38;
f2.$DOWN=40;
f2.$ENTER=13;
f2.$CANCEL=27;
f2.$PageUp=33;
f2.$PageDown=34;
f2.$Home=36;
f2.$End=35;
f2.$Tab=9;
FarPoint.System.WebControl={};
FarPoint.System.WebControl.MultiColumnComboBoxCellTypeUtilitis={};
var f3=FarPoint.System.WebControl.MultiColumnComboBoxCellTypeUtilitis;
var f4=f3.Consts={
$ATTRI_MULTICOMBO_PART_TYPE:"MccbPartType",
$ATTRI_LIST_ALIGNMENT:"MccbListAlignment",
$ATTRI_LIST_OFFSET:"MccbListOffset",
$ATTRI_LIST_WIDTH:"MccbListWidth",
$ATTRI_LIST_HEIGHT:"MccbListHEIGHT",
$ATTRI_COLUMN_EDIT:"MccbColumnEdit",
$ATTRI_COLUMN_DATA:"MccbColumnData",
$ATTRI_ID:"MccbId",
$ATTRI_LIST_MIN_HEIGHT:"MccbListMinHeight",
$ATTRI_LIST_MIN_WIDTH:"MccbListMinWidth",
$TYPE_DROPDOWNBUTTON:"DropDownButton",
$ID_BUTTON_OUTSIDE:"_DropDownButtonOutside",
$ID_BUTTON_INSIDE:"_DropDownButtonInside",
$ID_INPUT:"_Input",
$ID_CONTAINER:"_Container",
$ID_CONTAINER_DIV:"_ContainerDiv",
$ID_SPREAD:"_FpSpread",
$ID_STATUS_RESIZE:"_ResizeButton",
$OBJECT_SUFFIX:"_Obj"
};
f3.CloseAll=function (){
var f5=document.body.lastChild;
if (f5!=null&&f5.tagName!=null&&f5.tagName=="DIV"){
if (f5.id!=null&&f5.close&&f5.id.match(new RegExp(f4.$ID_CONTAINER_DIV+"$"))){
f5.close();
}
}
};
FarPoint.System.WebControl.MultiColumnComboBoxCellType=function (mc){
if (mc==null)return null;
var f6=true;
var f7=null;
var f8=false;
var f9=false;
var g0=-1;
var g1=0;
var g2=false;
var g3=0;
var g4=0;
var g5=50;
var g6=200;
var g7=false;
var g8=this;
this.Init=function (){
if (f6){
this.InitSpread();
this.setController();
var g9=0;
while (g9<12){
if (g9==0||g9==1||g9==3){
g9++;
continue ;
}
if (f7[g9]!=null&&f7[g9].event=="SelectionChanged"){
if (FarPoint.System.CheckBrowserByName("IE")){
this.getFpSpread().onSelectionChanged=f7[g9].handler;
g9++;
continue ;
}
}
this.SetHandler(f7,g9,0);
g9++;
}
var h0=this.getControl();
var h1=FarPoint.System.FindElementById(h0,f4.$ID_BUTTON_INSIDE,f4.$ATTRI_ID);
if (h0!=null&&h1!=null){
if (h0.offsetHeight-5>0)
h1.style.height=h0.offsetHeight-5;
}
this.setListWidth(parseInt(this.getControl().getAttribute(f4.$ATTRI_LIST_WIDTH)));
this.setListHeight(parseInt(this.getControl().getAttribute(f4.$ATTRI_LIST_HEIGHT)));
h0.Init=true;
f6=false;
}
}
this.Dispose=function (){
var h2=this.getController();
if (!h2)return ;
var g9=0;
while (f7[g9]!=null){
if (f7[g9].event=="SelectionChanged"){
if (FarPoint.System.CheckBrowserByName("IE")){
this.getFpSpread().onSelectionChanged=null;
g9++;
continue ;
}
}
this.SetHandler(f7,g9,1);
g9++;
}
}
this.getDragOffsetX=function (){
return g3;
}
this.setDragOffsetX=function (value){
g3=value;
}
this.getDragOffsetY=function (){
return g4;
}
this.setDragOffsetY=function (value){
g4=value;
}
this.getStatusBarHeight=function (){
return 13;
}
this.getIsDrag=function (){
return g2;
}
this.setIsDrag=function (value){
var h3=12;
g2=value;
if (g2){
this.SetHandler(f7,h3,0);
}else {
this.SetHandler(f7,h3,1);
}
}
this.getHostSpread=function (){
var h4=this.getFpSpread();
if (h4==null)return null;
var h5=FarPoint.System.CheckBrowserByName("IE")?h4.hostspread:h4.getAttribute("hostspread");
return document.getElementById(h5);
}
this.getControl=function (){
return mc;
}
this.getFpSpread=function (){
var h6=this.getContainer();
if (h6==null)return null;
return h6.getElementsByTagName("div")[0];
}
this.getContainer=function (){
var h0=this.getControl();
if (h0==null)return null;
return FarPoint.System.FindElementById(h0,f4.$ID_CONTAINER,f4.$ATTRI_ID);
}
this.getContainerDiv=function (){
var h0=this.getControl();
if (h0==null)return null;
return FarPoint.System.FindElementById(h0,f4.$ID_CONTAINER_DIV,f4.$ATTRI_ID);
}
this.getInputControl=function (){
var h0=this.getControl();
if (h0==null)return null;
return FarPoint.System.FindElementById(h0,f4.$ID_INPUT,f4.$ATTRI_ID);
}
this.getResizeButton=function (){
var h0=this.getControl();
if (h0==null)return null;
return FarPoint.System.FindElementById(h0,f4.$ID_STATUS_RESIZE,f4.$ATTRI_ID);
}
this.getController=function (){
return f7;
}
this.setController=function (){
if (f7==null){
f7={
1:{target:document,event:"mousedown",handler:function (event){g8.MouseDownOutside(event)},useCapture:false},
3:{target:document,event:"mouseup",handler:function (event){g8.MouseUpOutside(event)},useCapture:false},
12:{target:document,event:"mousemove",handler:function (event){g8.MouseMove(event)},useCapture:false},
4:{target:this.getControl(),event:"mousedown",handler:function (event){g8.MouseDown(event)},useCapture:false},
7:{target:this.getInputControl(),event:"keydown",handler:function (event){g8.OnInputKeyDown(event)},useCapture:FarPoint.System.CheckBrowserByName("IE")?false:true},
6:{target:this.getContainer(),event:"mousedown",handler:function (event){g8.CancelEvent(event)},useCapture:false},
9:{target:this.getFpSpread(),event:"SelectionChanged",handler:function (event){g8.OnSpreadSelectionChanged(event)},useCapture:false},
8:{target:this.getResizeButton(),event:"mousedown",handler:function (event){g8.ResizeButtonMouseDown(event)},useCapture:false}
};
if (FarPoint.System.CheckBrowserByName("IE")?typeof(this.getFpSpread().EnableClientScript)=="undefined":this.getFpSpread().getAttribute("EnableClientScript")==null){
f7[13]={target:FarPoint.System.CheckBrowserByName("IE")?this.getHostSpread():the_fpSpread.GetViewport(this.getHostSpread()).parentNode,event:"scroll",handler:function (event){g8.MccbctScroll(event)},useCapture:false};
}
}
}
this.getIsDrop=function (){
return f8;
}
this.setIsDrop=function (value){
f8=value;
}
this.getIsDroping=function (){
return f9;
}
this.setIsDroping=function (value){
f9=value;
}
this.getListAlignment=function (){
try {
var g9=this.getControl().getAttribute(f4.$ATTRI_LIST_ALIGNMENT);
return parseInt(g9);
}catch (exception ){
return 0;
}
}
this.getListOffset=function (){
try {
var g9=this.getControl().getAttribute(f4.$ATTRI_LIST_OFFSET);
return parseInt(g9);
}catch (exception ){
return 0;
}
}
this.getListWidth=function (){
return g5;
}
this.setListWidth=function (value){
if ((value<this.getListMinWidth())&&(value!=-1))
g5=this.getListMinWidth();
else {
if (!FarPoint.System.CheckBrowserByName("IE"))
if (value>2000)
value=2000;
g5=value;
}
}
this.getListHeight=function (){
return g6;
}
this.setListHeight=function (value){
if (value<this.getListMinHeight())
g6=this.getListMinHeight();
else 
g6=value;
}
this.getListMinWidth=function (){
try {
var g9=this.getControl().getAttribute(f4.$ATTRI_LIST_MIN_WIDTH);
return Math.min(Math.abs(parseInt(g9)),32767);
}catch (exception ){
return 50;
}
}
this.getListMinHeight=function (){
try {
var g9=this.getControl().getAttribute(f4.$ATTRI_LIST_MIN_HEIGHT);
return Math.min(Math.abs(parseInt(g9)),32767);
}catch (exception ){
return 50;
}
}
this.getEditColumnIndex=function (){
try {
var g9=this.getControl().getAttribute(f4.$ATTRI_COLUMN_EDIT);
return parseInt(g9);
}catch (exception ){
return 0;
}
}
this.setSelectedIndex=function (value){
g0=value;
}
this.getSelectedIndex=function (){
if (g0!=-1){
var h7=this.getFpSpread();
var h8=FarPoint.System.CheckBrowserByName("IE")?parseInt(h7.ActiveRow):parseInt(h7.GetActiveRow());
if (h7){
if (h8>=0){
this.setSelectedIndex(h8);
}
}
}
return g0;
}
this.setActiveColumnIndex=function (value){
g1=value;
}
this.getActiveColumnIndex=function (){
return g1;
}
this.getDataColumnIndex=function (){
try {
var g9=this.getControl().getAttribute(f4.$ATTRI_COLUMN_DATA);
return parseInt(g9);
}catch (exception ){
return 0;
}
}
this.FocusForEdit=function (){
var h0=this.getControl();
if (h0==null)return ;
if (h0.parentNode==null||typeof(h0.parentNode.tagName)=="undefined")return ;
if (h0.parentNode.tagName!="TD")return ;
if (h0.parentNode.getAttribute("FpCellType")!="MultiColumnComboBoxCellType")return ;
var h9=this.getInputControl();
if (h9!=null){
try {
h9.focus();
h9.select();
}catch (exception ){}
this.SetHandler(f7,7,1);
this.SetHandler(f7,7,0);
}
}
this.LockFocus=function (event){
var i0=this.getInputControl();
if (i0!=null&&typeof(i0.focus)!="undefined")
if (i0!=null){
try {
i0.focus();
i0.select();
}catch (exception ){}
}
}
this.MouseDown=function (event){
if (!FarPoint.System.CheckBrowserByName("IE")&&this.getControl().getAttribute("disabled")=="disabled")return FarPoint.System.CancelDefault(event);
if (!this.getIsDrop()&&event.button!=(FarPoint.System.CheckBrowserByName("IE")?1:0))return ;
var i1=FarPoint.System.GetTarget(event);
if (i1==null||i1.getAttribute(f4.$ATTRI_MULTICOMBO_PART_TYPE)!=f4.$TYPE_DROPDOWNBUTTON)return false;
this.setIsDroping(true);
var i2=this;
setTimeout(function (){i2.DropDown();},0);
}
this.DropDown=function (){
this.ShowHideContainer(!this.getIsDrop());
}
this.OnInputKeyDown=function (event){
if (event.altKey&&event.keyCode==f2.$DOWN){
this.ShowHideContainer(!this.getIsDrop());
FarPoint.System.CancelDefault(event);
return false;
}
switch (event.keyCode){
case f2.$UP:
if (this.getIsDrop()){
this.ChangeSelectedIndex(-1);
var i3=this.getFpSpread();
if (!FarPoint.System.CheckBrowserByName("IE")){
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),this.getActiveColumnIndex());
}else {
i3.ScrollTo(i3.ActiveRow,this.getActiveColumnIndex());
}
if (FarPoint.System.CheckBrowserByName("IE"))
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$LEFT:
if (this.getIsDrop()){
this.ChangedActiveColumnIndex(true);
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$DOWN:
if (this.getIsDrop()){
this.ChangeSelectedIndex(1);
var i3=this.getFpSpread();
if (!FarPoint.System.CheckBrowserByName("IE")){
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),this.getActiveColumnIndex());
}else {
i3.ScrollTo(i3.ActiveRow,this.getActiveColumnIndex());
}
if (FarPoint.System.CheckBrowserByName("IE"))
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$RIGHT:
if (this.getIsDrop()){
this.ChangedActiveColumnIndex(false);
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$ENTER:
if (this.getIsDrop()){
this.ShowHideContainer(false);
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("safari")&&this.getFpSpread().getAttribute("EnableClientScript")=="0"){
return FarPoint.System.CancelDefault(event);
}
break ;
case f2.$CANCEL:
if (this.getIsDrop()){
this.ShowHideContainer(false);
}
FarPoint.System.CancelDefault(event);
break ;
case f2.$PageUp:
if (this.getIsDrop()){
this.ChangeSelectedIndex(null,1);
var i3=this.getFpSpread();
if (!FarPoint.System.CheckBrowserByName("IE")){
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),this.getActiveColumnIndex());
}else {
i3.ScrollTo(i3.ActiveRow,this.getActiveColumnIndex());
}
if (FarPoint.System.CheckBrowserByName("IE"))
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$PageDown:
if (this.getIsDrop()){
this.ChangeSelectedIndex(null,2);
var i3=this.getFpSpread();
if (!FarPoint.System.CheckBrowserByName("IE")){
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),this.getActiveColumnIndex());
}else {
i3.ScrollTo(i3.ActiveRow,this.getActiveColumnIndex());
}
if (FarPoint.System.CheckBrowserByName("IE"))
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$Home:
if (this.getIsDrop()){
this.ChangeSelectedIndex(null,3);
var i3=this.getFpSpread();
if (!FarPoint.System.CheckBrowserByName("IE")){
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),this.getActiveColumnIndex());
}else {
i3.ScrollTo(i3.ActiveRow,this.getActiveColumnIndex());
}
if (FarPoint.System.CheckBrowserByName("IE"))
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$End:
if (this.getIsDrop()){
this.ChangeSelectedIndex(null,4);
var i3=this.getFpSpread();
if (!FarPoint.System.CheckBrowserByName("IE")){
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),this.getActiveColumnIndex());
}else {
i3.ScrollTo(i3.ActiveRow,this.getActiveColumnIndex());
}
if (FarPoint.System.CheckBrowserByName("IE"))
FarPoint.System.CancelDefault(event);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
}
break ;
case f2.$Tab:
if (this.getIsDrop()){
this.ShowHideContainer(false);
if (!FarPoint.System.CheckBrowserByName("IE")){
FarPoint.System.CancelDefault(event);
if (FarPoint.System.CheckBrowserByName("FF")){
var i4=this.getControl().getElementsByTagName("input")[1];
if (i4!=null){
var i5=document.createEvent('KeyboardEvent');
i5.initKeyEvent('keydown',true,true,null,null,null,null,null,9,null);
setTimeout(function (){i4.dispatchEvent(i5)},0);
}
}
}
}
break ;
}
}
this.OnSpreadSelectionChanged=function (event){
var h7=this.getFpSpread();
if (h7==null)return ;
if (!FarPoint.System.CheckBrowserByName("IE")){
this.setSelectedIndex(h7.GetActiveRow());
}else {
this.setSelectedIndex(h7.ActiveRow);
}
var h9=this.getInputControl();
if (h9==null)return ;
if (this.getSelectedIndex()>=0&&this.getEditColumnIndex()>=0){
h9.value=h7.GetValue(this.getSelectedIndex(),this.getEditColumnIndex());
h9.select();
}
FarPoint.System.GetEvent(event).cancelBubble=true;
}
this.CancelEvent=function (event){
if (this.getIsDrop()){
this.LockFocus(event);
var i6=this;
setTimeout(function (){i6.LockFocus(event);},0);
}
return FarPoint.System.CancelDefault(event);
}
this.MouseDownOutside=function (event){
if (this.getIsDrag())
this.setIsDrag(false);
var i7=this.getContainerDiv();
var i8=this.getControl();
var i3=this.getFpSpread();
var i9=document.getElementById(i3.id+"_viewport");
var i1=FarPoint.System.GetTarget(event);
if (!FarPoint.System.IsChild(i7,i1)&&!FarPoint.System.IsChild(i8,i1)){
this.setIsDroping(false);
if (this.getIsDrop())
this.ShowHideContainer(false);
}
var i6=this;
setTimeout(function (){i6.LockFocus(event);},0);
}
this.MouseUpOutside=function (event){
var i1=FarPoint.System.GetTarget(event);
var i8=this.getControl();
if (this.getIsDroping()&&FarPoint.System.IsChild(i8,i1)){
this.setIsDroping(false);
return ;
}
if (this.getIsDrop()==false)return ;
if (this.getIsDrag()){
this.setIsDrag(false);
return ;
}
var i7=this.getContainerDiv();
var i3=this.getFpSpread();
var i9=document.getElementById(i3.id+"_viewport");
if (FarPoint.System.IsChild(i9,i1)||!FarPoint.System.IsChild(i7,i1)){
this.ShowHideContainer(false);
}
var i6=this;
setTimeout(function (){i6.LockFocus(event);},0);
}
this.ResizeButtonMouseDown=function (event){
this.LockFocus(event);
if (event.button!=(FarPoint.System.CheckBrowserByName("IE")?1:0))return ;
var h6=this.getContainer();
if (h6==null)return ;
var j0=FarPoint.System.GetMouseCoords(event);
this.setDragOffsetX(parseInt(h6.offsetLeft+h6.offsetWidth)-j0.x);
this.setDragOffsetY(parseInt(h6.offsetTop+h6.offsetHeight)-j0.y);
this.setIsDrag(true);
if (event.preventDefault)event.preventDefault();
event.returnValue=false;
event.cancelBubble=true;
return false;
}
this.MouseMove=function (event){
if (!this.getIsDrag())return ;
var h6=this.getContainer();
if (h6==null)return ;
var j1=this.getContainerDiv();
if (j1==null)return ;
var j0=FarPoint.System.GetMouseCoords(event);
var j2=j0.x-parseInt(h6.offsetLeft)+this.getDragOffsetX();
var j3=j0.y-parseInt(h6.offsetTop)+this.getDragOffsetY()-5;
if (j2>this.getListMinWidth()&&Math.abs(j2-h6.offsetWidth)>5){
j1.style.width=j2+"px";
h6.style.width=j2+"px";
this.setListWidth(j2+5);
}
if (j3>this.getListMinHeight()&&Math.abs(j3-h6.offsetHeight)>20){
var i3=this.getFpSpread();
if (i3!=null)
i3.style.height=""+(j3-this.getStatusBarHeight())+"px";
j1.style.height=(j3+5)+"px";
h6.style.height=j3+"px";
this.setListHeight(j3);
}
if (!FarPoint.System.CheckBrowserByName("IE")){
var j4=this.getFpSpread();
the_fpSpread.SizeSpread(j4)
the_fpSpread.Refresh(j4);
}
event.cancelBubble=true;
return false;
}
this.MccbctScroll=function (event){
var h0=this.getControl();
if (h0==null)return ;
var h6=this.getContainer();
if (h6==null)return ;
var j1=this.getContainerDiv();
if (j1==null)return ;
if (FarPoint.System.CheckBrowserByName("safari")&&h0.offsetHeight==0){
j1.style.top=(FarPoint.System.GetOffsetTop(h0)+25)-(this.GetSpreadClientData(this.getHostSpread(),1))+"px";
}else {
var j5=null;
if (FarPoint.System.CheckBrowserByName("IE")?typeof(this.getFpSpread().EnableClientScript)=="undefined":this.getFpSpread().getAttribute("EnableClientScript")==null)
j5=FarPoint.System.CheckBrowserByName("IE")?document.getElementById(this.getHostSpread().id+"_view"):document.getElementById(this.getHostSpread().id+"_viewport").parentNode;
var j6=FarPoint.System.IsChild(j5,h0)?(this.GetSpreadClientData(this.getHostSpread(),1)):0;
j1.style.top=(FarPoint.System.GetOffsetTop(h0)+h0.offsetHeight+2)-j6+"px";
}
}
this.GetAdjustorForScroll=function (){
var j7=0;var j8=0;
var e7=new String(this.getControl().getAttribute(f4.$ATTRI_ID));
j7=parseInt(e7.split(new RegExp("_"))[1]);
j8=parseInt(e7.split(new RegExp("_"))[2]);
var j9=e7.split(new RegExp("_"))[3];
var k0=this.getHostSpread();
var k1=FarPoint.System.CheckBrowserByName("IE");
var k2=FarPoint.System.CheckBrowserByName("safari");
var k3={left:0,top:0};
if (this.getFpSpread().getAttribute("EnableClientScript")=="0")
return k3;
if (k2){
if (j9!="sc"&&j9!="rh")
k3.left=this.GetSpreadClientData(k0,0);
if (j9!="ch"&&j9!="cf"&&j9!="sc")
k3.top=this.GetSpreadClientData(k0,1);
return k3;
}
var k4=k1?k0.getViewport():the_fpSpread.GetViewport(k0);
var k5=k1?k0.getViewport0():the_fpSpread.GetViewport0(k0);
var k6=k1?k0.getViewport1():the_fpSpread.GetViewport1(k0);
var k7=k1?k0.getViewport2():the_fpSpread.GetViewport2(k0);
var k8=0;var k9=0;
k8=k1?(k6!=null?k6.rows.length:0):k0.frzRows;
if (k1){
if (k5!=null){
var l0=k5.getElementsByTagName("COLGROUP");
if (l0!=null&&l0.length>0)
k9=l0[0].childNodes.length;
}else if (k7!=null){
var l0=k7.getElementsByTagName("COLGROUP");
if (l0!=null&&l0.length>0)
k9=l0[0].childNodes.length;
}
}else {
k9=k0.frzCols;
}
if ((j9!="ch"&&j9!="cf"&&j9!="sc")&&((k8>0&&(j7+1)>k8)||k8==0))
k3.top=this.GetSpreadClientData(k0,1);
if ((j9!="sc"&&j9!="rh")&&((k9>0&&(j8+1)>k9)||k9==0))
k3.left=this.GetSpreadClientData(k0,0);
return k3;
}
this.InitSpread=function (){
if (!FarPoint.System.CheckBrowserByName("IE")&&typeof(the_fpSpread)!="undefined"){
var i3=this.getFpSpread();
the_fpSpread.Init(i3);
the_fpSpread.SizeAll(i3);
i3.dispose=function (){
the_fpSpread.Dispose(i3);
}
}
}
this.IsContained=function (child){
return FarPoint.System.IsChild(this.getControl(),child)||FarPoint.System.IsChild(this.getContainer(),child);
}
this.GetActivePositonInDomTree=function (element){
if (element==null)return false;
while (element!=null&&element!=document.body){
if (element.tagName=="TR"&&element.getAttribute("FpSpread")!=null)return element.getAttribute("FpSpread");
element=element.parentNode;
}
return "";
}
this.GetSpreadClientData=function (i3,whichData){
if (this.getFpSpread().getAttribute("EnableClientScript")=="0")return 0;
var l1="";
var l2=0;
var l3=null;
if (FarPoint.System.CheckBrowserByName("ie")){
if (i3.GetParentSpread()!=null)return ;
l3=document.getElementById(i3.id+"_XMLDATA");
switch (whichData){
case 0:
l1="/root/scrollLeft";
break ;
case 1:
l1="/root/scrollTop";
break ;
}
var l2=l3.documentElement.selectSingleNode(l1);
if (l2!=null&&l2.text!=""){
l2=parseInt(l2.text);
}
}else {
if (the_fpSpread.GetParentSpread(i3)!=null)return ;
l3=the_fpSpread.GetData(i3);
var l4=l3.getElementsByTagName("root")[0];
switch (whichData){
case 0:
l1="scrollLeft";
break ;
case 1:
l1="scrollTop";
break ;
}
var l5=l4.getElementsByTagName(l1)[0];
if (l5!=null&&l5.innerHTML!=""){
l2=parseInt(l5.innerHTML);
}
}
if (isNaN(l2))l2=0;
return l2;
}
this.SetHandler=function (f7,index,method){
if (isNaN(index)||index<0)return ;
if ((typeof(f7[index])=="undefined")||(f7[index]==null))return ;
switch (method){
case 0:
FarPoint.System.AttachEvent(f7[index].target,f7[index].event,f7[index].handler,f7[index].useCapture);
break ;
case 1:
FarPoint.System.DetachEvent(f7[index].target,f7[index].event,f7[index].handler,f7[index].useCapture);
break ;
}
}
this.ChangeSelectedIndex=function (step,caseId){
var l6=0;
if (typeof(caseId)!="undefined")
l6=caseId;
var i3=this.getFpSpread();
if (!i3)return ;
var l7=i3.GetRowCount();
if (l7<=0)return ;
var l8=this.getSelectedIndex();
var l9=this.getActiveColumnIndex();
if (l9<0)l9=0;
switch (l6){
case 0:
l8+=step;
if ((l8<0)||(l8>=l7))return ;
break ;
case 1:
l8-=5;
l8=Math.max(l8,0);
break ;
case 2:
l8+=5;
l8=Math.min(l8,l7-1);
break ;
case 3:
l8=0;
break ;
case 4:
l8=l7-1;
break ;
}
this.setSelectedIndex(l8);
i3.SetActiveCell(l8,l9);
}
this.ChangedActiveColumnIndex=function (IsLeft){
var i3=this.getFpSpread();
if (!i3)return ;
var m0=i3.GetColCount();
if (m0<=0)return ;
var m1=FarPoint.System.CheckBrowserByName("IE")?i3.ActiveRow:i3.GetActiveRow();
var m2=this.getActiveColumnIndex();
if (isNaN(m2))m2=0;
m2=IsLeft?m2-1:m2+1;
if ((m2<0)||(m2>=m0)){
m2=Math.max(m2,0);
m2=Math.min(m2,m0-1);
this.setActiveColumnIndex(m2);
return ;
}
if (FarPoint.System.CheckBrowserByName("IE"))
i3.ScrollTo(m1,m2);
else 
the_fpSpread.ScrollTo(i3,m1,m2);
this.setActiveColumnIndex(m2);
}
this.ShowHideContainer=function (show){
var h0=this.getControl();
if (h0==null)return ;
var h6=this.getContainer();
if (h6==null)return ;
var j1=this.getContainerDiv();
if (j1==null)return ;
if (!FarPoint.System.CheckBrowserByName("IE")){
j1.style.display=(show?'block':'none');
j1.style.visibility=(show?'visible':'hidden');
}
if (show){
h6.style.height=this.getListHeight()+"px";
j1.style.height=(this.getListHeight()+5)+"px";
h6.style.top=(-this.getListHeight()*0.25)+"px";
var k3=this.GetAdjustorForScroll();
if (FarPoint.System.CheckBrowserByName("safari")&&h0.offsetHeight==0){
j1.style.top=(FarPoint.System.GetOffsetTop(h0)+25)-k3.top+"px";
}else {
j1.style.top=(FarPoint.System.GetOffsetTop(h0)+h0.offsetHeight)-k3.top+"px";
}
var m3=FarPoint.System.GetOffsetLeft(h0);
if (this.getListAlignment()==0)
m3+=this.getListOffset();
else {
var m4=0;
if (this.getListWidth()!=-1)
m4=(this.getListWidth()-h0.parentNode.offsetWidth);
m3-=(this.getListOffset()+m4);
}
j1.style.left=m3-k3.left+"px";
var m5=this.getListWidth();
if (m5<0)m5=Math.max(this.getListMinWidth(),h0.parentNode.offsetWidth);
h6.style.width=(m5+5)+"px";
j1.style.width=(m5+5)+"px";
document.body.appendChild(j1);
var m6=this;
j1.close=function (){
m6.ShowHideContainer(false);
};
this.SetHandler(f7,1,0);
this.SetHandler(f7,3,0);
this.SetHandler(f7,13,0);
}else {
j1.close=null;
h0.appendChild(j1);
j1.style.top=-10000;
j1.style.left=-10000;
this.SetHandler(f7,1,1);
this.SetHandler(f7,3,1);
this.SetHandler(f7,13,1);
}
var i3=this.getFpSpread();
if (show&&i3!=null){
i3.style.height=(parseInt(h6.style.height)-this.getStatusBarHeight())+"px";
}
this.setIsDrop(show);
if (!FarPoint.System.CheckBrowserByName("IE")){
if (i3!=null){
var c6=the_fpSpread.GetColHeader(i3);
if (c6!=null&&FarPoint.System.CheckBrowserByName("Firefox")){
c6.parentNode.style.height=""+(c6.offsetHeight-parseInt(c6.cellSpacing))+"px";
}
the_fpSpread.SizeAll(i3);
the_fpSpread.SizeAll(i3);
if (show){
if (this.getFpSpread().getAttribute("EnableClientScript")!="0"){
the_fpSpread.SetPageActiveSpread(i3);
the_fpSpread.SetActiveSpreadID(i3,i3.id,i3.id,false);
}
var l8=this.getSelectedIndex();
i3.SetActiveCell(l8,0);
this.setActiveColumnIndex(0);
if (i3.GetActiveRow()>-1)
the_fpSpread.ScrollTo(i3,i3.GetActiveRow(),0);
}else {
var m7=this.getHostSpread();
if (this.getFpSpread().getAttribute("EnableClientScript")!="0"){
the_fpSpread.SetPageActiveSpread(m7);
the_fpSpread.SetActiveSpreadID(m7,m7.id,m7.id,false);
}
}
}
}else {
if (show){
var l8=this.getSelectedIndex();
i3.SetActiveCell(l8,0);
this.setActiveColumnIndex(0);
if (i3.ActiveRow>-1)
i3.ScrollTo(i3.ActiveRow,0);
}
}
if (show){
var i2=this;
setTimeout(function (){i2.AnimShow(h6);},30);
}
var h9=this.getInputControl();
if (h9!=null){
try {
h9.focus();
h9.select();
if (h9.value.length>0&&this.getSelectedIndex()==-1){
this.SetText(h9.value);
}
}catch (exception ){}
}
if (FarPoint.System.CheckBrowserByName("IE")){
j1.style.display=(show?'block':'none');
j1.style.visibility=(show?'visible':'hidden');
}
}
this.AnimShow=function (h6){
var m8=h6.offsetTop;
if (m8>=0)return ;
var m9=m8<-5?m8*0.25:0;
h6.style.top=m9+"px";
var i2=this;
setTimeout(function (){i2.AnimShow(h6);},30);
}
this.SetText=function (text){
var i3=this.getFpSpread();
if (i3==null)return ;
var n0=this.getEditColumnIndex();
var h9=this.getInputControl();
if (text==null){
if (h9!=null){
h9.value="";
this.setSelectedIndex(-1);
}
i3.SetActiveCell(-1,-1);
return ;
}
var n1=text.match(new RegExp("^\\s*(\\S+(\\s+\\S+)*)\\s*$"));
text=(n1==null)?"":n1[1];
if ((text.length==1&&text.charCodeAt(0)==160)||(text=="")||(text.length<=0)){
if (h9!=null){
h9.value="";
this.setSelectedIndex(-1);
}
i3.SetActiveCell(-1,-1);
return ;
}
if ((!FarPoint.System.CheckBrowserByName("IE"))&&typeof(i3.GetRowCount)=="undefined"){
var g9=0,l8=-1;
while ((g9<the_fpSpread.spreads.length)&&(l8==-1)){
if (the_fpSpread.spreads[g9].id==i3.id)l8=g9;
g9++;
}
the_fpSpread.spreads.splice(l8,1);
the_fpSpread.Init(document.getElementById(i3.id));
document.getElementById(i3.id).dispose=function (){
the_fpSpread.Dispose(document.getElementById(i3.id));
}
}
if (!FarPoint.System.CheckBrowserByName("IE")){
text=text.replace(new RegExp("\xA0","g"),String.fromCharCode(32));
}
var n2=i3.GetRowCount();
var l8=0;
for (;l8<n2;l8++){
try {
var n3=i3.GetValue(l8,n0);
if (n3==(FarPoint.System.CheckBrowserByName("IE")?text:the_fpSpread.Trim(text))){
i3.SetActiveCell(l8,n0);
break ;
}
}catch (exception ){
return ;
}
}
}
this.TestProps=function (){
if (!f2.$FLAG_ISDEBUG)return ;
}
this.Init();
}
FarPoint.System.WebControl.MultiColumnComboBoxCellType.CheckInit=function (id){
var h0=document.getElementById(id);
if (h0==null){
h0=document.getElementById(id+"Editor");
id+="Editor";
}
if (h0&&h0.Init)return ;
try {
var n4=eval(id+f4.$OBJECT_SUFFIX);
if (n4){
n4.Dispose();
delete n4;
}
}catch (exception ){}
var n5=id+f4.$OBJECT_SUFFIX+"=new FarPoint.System.WebControl.MultiColumnComboBoxCellType(document.getElementById('"+id+"'));";
eval(n5);
}
FarPoint.System._ExtenderHelper=function (){
this.ScriptHolderID="__PAGESCRIPT";
this.ScriptBlockID="__SCRIPTBLOCK";
this.StartupScriptID="__STARTUPSCRIPT";
this.CssLinksID="__CSSLINKS";
}
FarPoint.System._ExtenderHelper.prototype={
getExtenderScripts:function (){
var n6={};
var n7=document.getElementsByTagName("input");
for (var g9=0;g9<n7.length;g9++){
var n8=n7[g9].id.match(new RegExp("^(.+)_extender$"));
if (n8&&n8.length==2){
var n9=n8[1];
var o0=$get(n9);
if (o0&&(o0.FpSpread=="Spread"||o0.getAttribute("FpSpread")=="Spread")){
var o1=n7[g9].json||n7[g9].getAttribute("json");
var o2=eval("("+o1+")");
this.mergeExtenderScripts(n6,o2.extenderScripts);
}
}
}
return n6;
},
mergeExtenderScripts:function (i1,source){
for (var o3 in source){
var n3=source[o3];
if (!i1[o3]){
i1[o3]=n3;
}else {
for (var o4=0;o4<n3.length;o4++){
var o5=n3[o4];
if (!Array.contains(i1[o3],o5))i1[o3].push(o5);
}
}
}
},
getNeededExtenderScripts:function (newScripts,realScripts,loadedScripts){
var o6=[];
var o7=[];
for (var o3 in newScripts){
Array.addRange(o6,newScripts[o3]);
}
for (var o3 in loadedScripts){
Array.addRange(o7,loadedScripts[o3]);
}
var o8=[];
for (var g9=0;g9<realScripts.length;g9++){
var n3=realScripts[g9];
if (!Array.contains(o7,n3)&&Array.contains(o6,n3))
o8.push(n3);
}
return o8;
},
get_scriptHolder:function (){
var o9=$get(this.ScriptHolderID);
if (!o9){
o9=document.createElement("div");
o9.id=this.ScriptHolderID;
o9.style.display="none";
document.body.appendChild(o9);
}
return o9;
},
saveLoadedExtenderScripts:function (i3){
var o9=this.get_scriptHolder();
o9.innerHTML="";
var p0=this.getExtenderScripts();
var o7=o9.loaded;
if (!o7)o7={};
this.mergeExtenderScripts(o7,p0);
o9.loaded=o7;
if (typeof(FpExtender)!='undefined')FpExtender.Util.disposeExtenders(i3);
},
processCss:function (buff){
var p1=document.getElementsByTagName("head");
if (!p1){
p1=document.createElement("head");
document.documentElement.insertBefore(p1,document.body)
}else {
p1=p1[0];
}
var p2=[];
var p3=p1.getElementsByTagName("link");
if (p3){
for (var g9=0;g9<p3.length;g9++){
var p4=p3[g9];
if (p4.getAttribute("type")=="text/css"){
p2.push(p4.getAttribute("href"));
}
}
}
var p5=$get(this.CssLinksID,buff);
if (p5){
p5=eval("("+p5.value+")");
p5=p5.cssLinks;
for (var g9=0;g9<p5.length;g9++){
var p6=p5[g9];
if (!Array.contains(p2,p6)){
var p4=document.createElement("link");
p4.type="text/css";
p4.rel="stylesheet";
p4.href=p6;
p1.appendChild(p4);
}
}
}
},
loadExtenderScripts:function (i3,buff){
if (Sys.Browser.agent!=Sys.Browser.InternetExplorer)
this.processCss(buff);
var p7=[];
var p8=[];
var p9=$get(this.ScriptBlockID,buff);
if (p9){
var q0=new RegExp("<script src=\"(.+)\" type=\"text\\/javascript\"><\\/script>","gm");
var q1;
while ((q1=q0.exec(p9.value))!=null){
p8.push(q1[1]);
}
}
var q2=this.getExtenderScripts();
var o9=this.get_scriptHolder();
var o8=this.getNeededExtenderScripts(q2,p8,o9.loaded);
for (var g9=0;g9<o8.length;g9++)p7.push({src:o8[g9]});
var q3=$get(this.StartupScriptID,buff);
var q4=Sys._ScriptLoader.getInstance();
for (var g9=0;g9<p7.length;g9++){
var q5=p7[g9].src;
if (q5)q4.queueScriptReference(q5);
}
q4.loadScripts(0,function (){
if (q3&&typeof(FpExtender)!=='undefined'){
var q6=false;
var q7=FpExtender.Util.getExtenderInitScripts(i3,q3.value);
for (var g9=0;g9<q7.length;g9++){
eval(q7[g9]);
if (q7[g9].indexOf("Sys.Application.initialize")!=-1)q6=true;
}
if (!q6){
var q8=Sys.Application.getComponents();
for (var g9 in q8){
if (q8[g9].get_id().indexOf(i3.id)==0&&FpExtender.ContainerBehavior.isInstanceOfType(q8[g9]))
q8[g9]._load();
}
}
}
},function (){
},null);
}
}
FarPoint.System.ExtenderHelper=new FarPoint.System._ExtenderHelper();
