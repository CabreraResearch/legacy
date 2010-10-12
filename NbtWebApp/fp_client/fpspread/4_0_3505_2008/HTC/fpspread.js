//
//	Copyright?2005. FarPoint Technologies.	All rights reserved.
//
var the_fpSpread = new Fpoint_FPSpread();
function FpSpread_EventHandlers(){
var e3=the_fpSpread;
this.TranslateKeyPress=function (event){
e3.TranslateKeyPress(event);
}
this.TranslateKey=function (event){
e3.TranslateKey(event);
}
this.SetActiveSpread=function (event){
e3.SetActiveSpread(event);
}
this.MouseDown=function (event){
e3.MouseDown(event);
}
this.MouseUp=function (event){
e3.MouseUp(event);
}
this.MouseMove=function (event){
e3.MouseMove(event);
}
this.DblClick=function (event){
e3.DblClick(event);
}
this.HandleFirstKey=function (event){
e3.HandleFirstKey(event);
}
this.DoPropertyChange=function (event){
e3.DoPropertyChange(event);
}
this.CmdbarMouseOver=function (event){
e3.CmdbarMouseOver(event);
}
this.CmdbarMouseOut=function (event){
e3.CmdbarMouseOut(event);
}
this.ScrollViewport=function (event){
e3.ScrollViewport(event);
}
this.Focus=function (event){
var e4=event.target;
e3.Focus(e4);
}
var e5=window.navigator.userAgent;
var e6=(e5.indexOf("Firefox/3.")>=0);
if (e6)
e3.AttachEvent(document,"keypress",this.TranslateKeyPress,true);
e3.AttachEvent(document,"keydown",this.TranslateKey,true);
e3.AttachEvent(document,"mousedown",this.SetActiveSpread,false);
e3.AttachEvent(document,"keyup",this.HandleFirstKey,true);
e3.AttachEvent(window,"resize",e3.DoResize,false);
this.AttachEvents=function (e4){
e3.AttachEvent(e4,"mousedown",this.MouseDown,false);
e3.AttachEvent(e4,"mouseup",this.MouseUp,false);
e3.AttachEvent(document,"mouseup",this.MouseUp,false);
e3.AttachEvent(e4,"mousemove",this.MouseMove,false);
e3.AttachEvent(e4,"dblclick",this.DblClick,false);
e3.AttachEvent(e4,"focus",this.Focus,false);
var e7=e3.GetViewport(e4);
if (e7!=null){
e3.AttachEvent(e3.GetViewport(e4).parentNode,"DOMAttrModified",this.DoPropertyChange,true);
e3.AttachEvent(e3.GetViewport(e4).parentNode,"scroll",this.ScrollViewport);
}
var e8=e3.GetCommandBar(e4);
if (e8!=null){
e3.AttachEvent(e8,"mouseover",this.CmdbarMouseOver,false);
e3.AttachEvent(e8,"mouseout",this.CmdbarMouseOut,false);
}
}
this.DetachEvents=function (e4){
e3.DetachEvent(e4,"mousedown",this.MouseDown,false);
e3.DetachEvent(e4,"mouseup",this.MouseUp,false);
e3.DetachEvent(document,"mouseup",this.MouseUp,false);
e3.DetachEvent(e4,"mousemove",this.MouseMove,false);
e3.DetachEvent(e4,"dblclick",this.DblClick,false);
e3.DetachEvent(e4,"focus",this.Focus,false);
var e7=e3.GetViewport(e4);
if (e7!=null){
e3.DetachEvent(e3.GetViewport(e4).parentNode,"DOMAttrModified",this.DoPropertyChange,true);
e3.DetachEvent(e3.GetViewport(e4).parentNode,"scroll",this.ScrollViewport);
}
var e8=e3.GetCommandBar(e4);
if (e8!=null){
e3.DetachEvent(e8,"mouseover",this.CmdbarMouseOver,false);
e3.DetachEvent(e8,"mouseout",this.CmdbarMouseOut,false);
}
}
}
function Fpoint_FPSpread(){
this.a6=false;
this.a7=false;
this.a8=null;
this.a9=null;
this.b0=null;
this.b1=-1;
this.b2=null;
this.b3=null;
this.b4=null;
this.b5=null;
this.b6=-1;
this.b7=-1;
this.b8=null;
this.b9=null;
this.c0=new Array();
this.error=false;
this.InitFields=function (e4){
if (this.b3==null)
this.b3=new this.Margin();
e4.c8=null;
e4.groupBar=null;
e4.c9=null;
e4.d0=null;
e4.d1=null;
e4.d2=null;
e4.d3=null;
e4.d4=null;
e4.d5=null;
e4.d6=null;
e4.d7="";
e4.d8=null;
e4.e2=false;
e4.slideLeft=0;
e4.slideRight=0;
e4.setAttribute("rowCount",0);
e4.setAttribute("colCount",0);
e4.d9=new Array();
e4.e0=new Array();
e4.e1=new Array();
this.activePager=null;
this.dragSlideBar=false;
e4.allowColMove=(e4.getAttribute("colMove")=="true");
e4.allowGroup=(e4.getAttribute("allowGroup")=="true");
e4.selectedCols=[];
e4.msgList=new Array();
e4.mouseY=null;
e4.copymulticol=false;
}
this.RegisterSpread=function (e4){
var e9=this.GetTopSpread(e4);
if (e9!=e4)return ;
if (this.spreads==null){
this.spreads=new Array();
}
var f0=this.spreads.length;
for (var f1=0;f1<f0;f1++){
if (this.spreads[f1]==e4)return ;
}
this.spreads.length=f0+1;
this.spreads[f0]=e4;
}
this.Init=function (e4){
if (e4==null)alert("spread is not defined!");
e4.initialized=false;
this.b2=null;
this.c0=new Array();
this.RegisterSpread(e4);
this.InitFields(e4);
this.InitMethods(e4);
e4.c1=document.getElementById(e4.id+"_XMLDATA");
if (e4.c1==null){
e4.c1=document.createElement('XML');
e4.c1.id=e4.id+"_XMLDATA";
e4.c1.style.display="none";
document.body.insertBefore(e4.c1,null);
}
var f2=document.getElementById(e4.id+"_data");
if (f2!=null&&f2.getAttribute("data")!=null){
e4.c1.innerHTML=f2.getAttribute("data");
f2.value="";
}
this.SaveData(e4);
e4.c2=document.getElementById(e4.id+"_viewport");
if (e4.c2!=null){
e4.c3=e4.c2.parentNode;
}
e4.frozColHeader=document.getElementById(e4.id+"_frozColHeader");
e4.frozRowHeader=document.getElementById(e4.id+"_frozRowHeader");
e4.viewport0=document.getElementById(e4.id+"_viewport0");
e4.viewport1=document.getElementById(e4.id+"_viewport1");
e4.viewport2=document.getElementById(e4.id+"_viewport2");
e4.c4=document.getElementById(e4.id+"_corner");
if (e4.c4!=null&&e4.c4.childNodes.length>0){
e4.c4=e4.c4.getElementsByTagName("TABLE")[0];
}
e4.frzRows=e4.frzCols=0;
if (e4.viewport1!=null){
e4.frzRows=e4.viewport1.rows.length;
}
if (e4.viewport0!=null){
var f3=this.GetColGroup(e4.viewport0);
if (f3!=null)e4.frzCols=f3.childNodes.length;
}else if (e4.viewport2!=null){
var f3=this.GetColGroup(e4.viewport2);
if (f3!=null)e4.frzCols=f3.childNodes.length;
}
e4.c5=document.getElementById(e4.id+"_rowHeader");
if (e4.c5!=null)e4.c5=e4.c5.getElementsByTagName("TABLE")[0];
e4.c6=document.getElementById(e4.id+"_colHeader");
if (e4.c6!=null)e4.c6=e4.c6.getElementsByTagName("TABLE")[0];
e4.frozColFooter=document.getElementById(e4.id+"_frozColFooter");
e4.colFooter=document.getElementById(e4.id+"_colFooter");
if (e4.colFooter!=null)e4.colFooter=e4.colFooter.getElementsByTagName("TABLE")[0];
e4.footerCorner=document.getElementById(e4.id+"_footerCorner");
if (e4.footerCorner!=null&&e4.footerCorner.childNodes.length>0){
e4.footerCorner=e4.footerCorner.getElementsByTagName("TABLE")[0];
}
if (e4.frozColFooter!=null)e4.frozColFooter=e4.frozColFooter.getElementsByTagName("TABLE")[0];
var c7=e4.c7=document.getElementById(e4.id+"_commandBar");
if (e4.frozRowHeader!=null)e4.frozRowHeader=e4.frozRowHeader.getElementsByTagName("TABLE")[0];
if (e4.frozColHeader!=null)e4.frozColHeader=e4.frozColHeader.getElementsByTagName("TABLE")[0];
var f4=this.GetViewport(e4);
if (f4!=null){
e4.setAttribute("rowCount",f4.rows.length);
if (f4.rows.length==1)e4.setAttribute("rowCount",0);
e4.setAttribute("colCount",f4.getAttribute("cols"));
}
var d9=e4.d9;
var e1=e4.e1;
var e0=e4.e0;
this.InitSpan(e4,this.GetViewport0(e4),d9);
this.InitSpan(e4,this.GetViewport1(e4),d9);
this.InitSpan(e4,this.GetViewport2(e4),d9);
this.InitSpan(e4,this.GetViewport(e4),d9);
this.InitSpan(e4,this.GetColHeader(e4),e1);
this.InitSpan(e4,this.GetFrozColHeader(e4),e1);
this.InitSpan(e4,this.GetRowHeader(e4),e0);
this.InitSpan(e4,this.GetFrozRowHeader(e4),e0);
if (e4.frzRows!=0||e4.frzCols!=0){
var f5=0;
if (this.GetViewport1(e4)!=null)f5+=this.GetViewport1(e4).rows.length;
if (this.GetViewport(e4)!=null)f5+=this.GetViewport(e4).rows.length;
e4.setAttribute("rowCount",f5);
}
e4.style.overflow="hidden";
if (this.GetParentSpread(e4)==null){
this.LoadScrollbarState(e4);
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var f8=f7.getElementsByTagName("activespread")[0];
if (f8!=null&&f8.innerHTML!=""){
this.SetPageActiveSpread(document.getElementById(this.Trim(f8.innerHTML)));
}
}
this.InitLayout(e4);
e4.e2=true;
if (this.GetPageActiveSpread()==e4&&(e4.getAttribute("AllowInsert")=="false"||e4.getAttribute("IsNewRow")=="true")){
var f9=this.GetCmdBtn(e4,"Insert");
this.UpdateCmdBtnState(f9,true);
f9=this.GetCmdBtn(e4,"Add");
this.UpdateCmdBtnState(f9,true);
}
this.CreateTextbox(e4);
this.CreateFocusBorder(e4);
this.InitSelection(e4);
e4.initialized=true;
if (this.GetPageActiveSpread()==e4)
{
try {
this.Focus(e4);
}catch (e){}
}
this.SaveData(e4);
if (this.handlers==null)
this.handlers=new FpSpread_EventHandlers();
this.handlers.DetachEvents(e4);
this.handlers.AttachEvents(e4);
if (c7!=null&&e4.style.position==""){
c7.parentNode.style.backgroundColor=c7.style.backgroundColor;
c7.parentNode.style.borderTop=c7.style.borderTop;
}
this.CreateSizebar(e4);
this.SyncColSelection(e4);
}
this.Dispose=function (e4){
if (this.handlers==null)
this.handlers=new FpSpread_EventHandlers();
this.handlers.DetachEvents(e4);
}
this.CmdbarMouseOver=function (event){
var g0=this.GetTarget(event);
if (g0!=null&&g0.tagName=="IMG"&&g0.getAttribute("disabled")!="true"){
g0.style.backgroundColor="cyan";
}
}
this.CmdbarMouseOut=function (event){
var g0=this.GetTarget(event);
if (g0!=null&&g0.tagName=="IMG"){
g0.style.backgroundColor="";
}
}
this.DoPropertyChange=function (event){
if (event.attrName=="curpos"){
this.ScrollViewport(event);
}else if (this.b4==null&&this.b5==null&&event.attrName=="pageincrement"&&event.ctrlKey){
var e4=this.GetSpread(this.GetTarget(event));
if (e4!=null)
this.SizeAll(this.GetTopSpread(e4));
}
}
this.HandleFirstKey=function (){
var e4=this.GetPageActiveSpread();
if (e4==null)return ;
var e9=this.GetTopSpread(e4);
var g1=document.getElementById(e9.id+"_textBox");
if (g1!=null&&g1.value!=""){
var e5=window.navigator.userAgent;
var e6=(e5.indexOf("Firefox/3.")>=0);
if (e6&&this.a8!=null)
this.a8.value=this.a8.value+g1.value;
g1.value="";
}
}
this.IsXHTML=function (e4){
var e9=this.GetTopSpread(e4);
var g2=e9.getAttribute("strictMode");
return (g2!=null&&g2=="true");
}
this.GetData=function (e4){
return e4.c1;
}
this.AttachEvent=function (target,event,handler,useCapture){
if (target.addEventListener!=null){
target.addEventListener(event,handler,useCapture);
}else if (target.attachEvent!=null){
target.attachEvent("on"+event,handler);
}
}
this.DetachEvent=function (target,event,handler,useCapture){
if (target.removeEventListener!=null){
target.removeEventListener(event,handler,useCapture);
}else if (target.detachEvent!=null){
target.detachEvent("on"+event,handler);
}
}
this.CancelDefault=function (e){
if (e.preventDefault!=null){
e.preventDefault();
e.stopPropagation();
}else {
e.cancelBubble=false;
e.returnValue=false;
}
return false;
}
this.CreateEvent=function (name){
var g3=document.createEvent("Events")
g3.initEvent(name,true,true);
return g3;
}
this.Refresh=function (e4){
var g0=e4.style.display;
e4.style.display="none";
e4.style.display=g0;
}
this.InitMethods=function (e4){
var e3=this;
e4.Edit=function (){e3.Edit(this);}
e4.Update=function (){e3.Update(this);}
e4.Cancel=function (){e3.Cancel(this);}
e4.Clear=function (){e3.Clear(this);}
e4.Copy=function (){e3.Copy(this);}
e4.Paste=function (){e3.Paste(this);}
e4.Prev=function (){e3.Prev(this);}
e4.Next=function (){e3.Next(this);}
e4.Add=function (){e3.Add(this);}
e4.Insert=function (){e3.Insert(this);}
e4.Delete=function (){e3.Delete(this);}
e4.Print=function (){e3.Print(this);}
e4.StartEdit=function (cell){e3.StartEdit(this,cell);}
e4.EndEdit=function (){e3.EndEdit(this);}
e4.ClearSelection=function (){e3.ClearSelection(this);}
e4.GetSelectedRange=function (){return e3.GetSelectedRange(this);}
e4.SetSelectedRange=function (r,c,rc,cc,innerRow){e3.SetSelectedRange(this,r,c,rc,cc,innerRow);}
e4.GetSelectedRanges=function (){return e3.GetSelectedRanges(this);}
e4.AddSelection=function (r,c,rc,cc,innerRow){e3.AddSelection(this,r,c,rc,cc,innerRow);}
e4.AddSpan=function (r,c,rc,cc,spans){e3.AddSpan(this,r,c,rc,cc,spans);}
e4.RemoveSpan=function (r,c,spans){e3.RemoveSpan(this,r,c,spans);}
e4.GetActiveRow=function (){var g0=e3.GetRowFromCell(this,this.d1);if (g0<0)return g0;return e3.GetSheetIndex(this,g0);}
e4.GetActiveCol=function (){return e3.GetColFromCell(this,this.d1);}
e4.SetActiveCell=function (r,c){e3.SetActiveCell(this,r,c);}
e4.GetCellByRowCol=function (r,c){return e3.GetCellByRowCol(this,r,c);}
e4.GetValue=function (r,c){return e3.GetValue(this,r,c);}
e4.SetValue=function (r,c,v,noEvent,recalc){e3.SetValue(this,r,c,v,noEvent,recalc);}
e4.GetFormula=function (r,c){return e3.GetFormula(this,r,c);}
e4.SetFormula=function (r,c,f,recalc,clientOnly){e3.SetFormula(this,r,c,f,recalc,clientOnly);}
e4.GetHiddenValue=function (r,colName){return e3.GetHiddenValue(this,r,colName);}
e4.GetSheetRowIndex=function (r){return e3.GetSheetRowIndex(this,r);}
e4.GetSheetColIndex=function (c,innerRow){return e3.GetSheetColIndex(this,c,innerRow);}
e4.GetRowCount=function (){return e3.GetRowCount(this);}
e4.GetColCount=function (){return e3.GetColCount(this);}
e4.GetRowByKey=function (key){return e3.GetRowByKey(this,key);}
e4.GetColByKey=function (key){return e3.GetColByKey(this,key);}
e4.GetRowKeyFromRow=function (r){return e3.GetRowKeyFromRow(this,r);}
e4.GetColKeyFromCol=function (c){return e3.GetColKeyFromCol(this,c);}
e4.GetTotalRowCount=function (){return e3.GetTotalRowCount(this);}
e4.GetPageCount=function (){return e3.GetPageCount(this);}
e4.GetParentSpread=function (){return e3.GetParentSpread(this);}
e4.GetChildSpread=function (r,ri){return e3.GetChildSpread(this,r,ri);}
e4.GetChildSpreads=function (){return e3.GetChildSpreads(this);}
e4.GetParentRowIndex=function (){return e3.GetParentRowIndex(this);}
e4.GetActiveChildSheetView=function (){return e3.GetActiveChildSheetView(this);}
e4.GetSpread=function (g0){return e3.GetSpread(g0);}
e4.UpdatePostbackData=function (){e3.UpdatePostbackData(this);}
e4.SizeToFit=function (c){e3.SizeToFit(this,c);}
e4.SetColWidth=function (c,w){e3.SetColWidth(this,c,w);}
e4.GetPreferredRowHeight=function (r){return e3.GetPreferredRowHeight(this,r);}
e4.SetRowHeight2=function (r,h){e3.SetRowHeight2(this,r,h);}
e4.CallBack=function (cmd,asyncCallBack){e3.SyncData(this.getAttribute("name"),cmd,this,asyncCallBack);}
e4.AddKeyMap=function (keyCode,ctrl,shift,alt,action){e3.AddKeyMap(this,keyCode,ctrl,shift,alt,action);}
e4.RemoveKeyMap=function (keyCode,ctrl,shift,alt){e3.RemoveKeyMap(this,keyCode,ctrl,shift,alt);}
e4.MoveToPrevCell=function (){e3.MoveToPrevCell(this);}
e4.MoveToNextCell=function (){e3.MoveToNextCell(this);}
e4.MoveToNextRow=function (){e3.MoveToNextRow(this);}
e4.MoveToPrevRow=function (){e3.MoveToPrevRow(this);}
e4.MoveToFirstColumn=function (){e3.MoveToFirstColumn(this);}
e4.MoveToLastColumn=function (){e3.MoveToLastColumn(this);}
e4.ScrollTo=function (r,c){e3.ScrollTo(this,r,c);}
e4.focus=function (){e3.Focus(this);}
e4.SizeAll=function (){e3.SizeAll(this);}
e4.ShowMessage=function (msg,r,c,time){return e3.ShowMessage(this,msg,r,c,time);}
e4.HideMessage=function (r,c){return e3.HideMessage(this,r,c);}
e4.ProcessKeyMap=function (event){
if (this.keyMap!=null){
var f0=this.keyMap.length;
for (var f1=0;f1<f0;f1++){
var g4=this.keyMap[f1];
if (event.keyCode==g4.key&&event.ctrlKey==g4.ctrl&&event.shiftKey==g4.shift&&event.altKey==g4.alt){
var g5=false;
if (typeof(g4.action)=="function")
g5=g4.action();
else 
g5=eval(g4.action);
return g5;
}
}
}
return true;
}
e4.Cells=function (r,c){return e3.Cells(this,r,c);}
e4.Rows=function (r,c){return e3.Rows(this,r,c);}
e4.Columns=function (r,c){return e3.Columns(this,r,c);}
e4.GetTitleInfo=function (r,c){return e3.GetTitleInfo(this,r,c);}
e4.SizeSpread=function (e4){return e3.SizeSpread(e4);}
}
this.CreateTextbox=function (e4){
var e9=this.GetTopSpread(e4);
var g1=document.getElementById(e9.id+"_textBox");
if (g1==null)
{
g1=document.createElement('INPUT');
g1.type="text";
g1.setAttribute("autocomplete","off");
g1.style.position="absolute";
g1.style.borderWidth=0;
g1.style.top="-10px";
g1.style.left="-100px";
g1.style.width="0px";
g1.style.height="1px";
if (e4.tabIndex!=null)
g1.tabIndex=e4.tabIndex;
g1.id=e4.id+"_textBox";
e4.insertBefore(g1,e4.firstChild);
}
}
this.CreateSizebar=function (e4){
e4.sizeBar=document.getElementById(e4.id+"_sizeBar");
if (e4.sizeBar==null&&(e4.frzRows>0||e4.frzCols>0))
{
e4.sizeBar=document.createElement("img");
e4.sizeBar.style.position="absolute";
e4.sizeBar.style.borderWidth=1;
e4.sizeBar.style.top="0px";
e4.sizeBar.style.left="-400px";
e4.sizeBar.style.width="2px";
e4.sizeBar.style.height="400px";
e4.sizeBar.style.background="black";
e4.sizeBar.id=e4.id+"_sizeBar";
var g6=this.GetViewport(e4).parentNode;
g6.insertBefore(e4.sizeBar,null);
}
}
this.CreateLineBorder=function (e4,id){
var g7=document.getElementById(id);
if (g7==null)
{
g7=document.createElement('div');
g7.style.position="absolute";
g7.style.left="-1000px";
g7.style.top="0px";
g7.style.overflow="hidden";
g7.style.border="1px solid black";
if (e4.getAttribute("FocusBorderColor")!=null)
g7.style.borderColor=e4.getAttribute("FocusBorderColor");
if (e4.getAttribute("FocusBorderStyle")!=null)
g7.style.borderStyle=e4.getAttribute("FocusBorderStyle");
g7.id=id;
var g6=this.GetViewport(e4).parentNode;
g6.insertBefore(g7,null);
}
return g7;
}
this.CreateFocusBorder=function (e4){
if (e4.frzRows>0||e4.frzCols>0)return ;
if (this.GetTopSpread(e4).getAttribute("hierView")=="true")return ;
if (this.GetTopSpread(e4).getAttribute("showFocusRect")=="false")return ;
if (this.GetViewport(e4)==null)return ;
var g7=this.CreateLineBorder(e4,e4.id+"_focusRectT");
g7.style.height=0;
g7=this.CreateLineBorder(e4,e4.id+"_focusRectB");
g7.style.height=0;
g7=this.CreateLineBorder(e4,e4.id+"_focusRectL");
g7.style.width=0;
g7=this.CreateLineBorder(e4,e4.id+"_focusRectR");
g7.style.width=0;
}
this.GetPosIndicator=function (e4){
var g8=e4.posIndicator;
if (g8==null)
g8=this.CreatePosIndicator(e4);
else if (g8.parentNode!=e4)
e4.insertBefore(g8,null);
return g8;
}
this.CreatePosIndicator=function (e4){
var g8=document.createElement("img");
g8.style.position="absolute";
g8.style.top="0px";
g8.style.left="-400px";
g8.style.width="10px";
g8.style.height="10px";
g8.style.zIndex=1000;
g8.id=e4.id+"_posIndicator";
if (e4.getAttribute("clienturl")!=null)
g8.src=e4.getAttribute("clienturl")+"down.gif";
else 
g8.src=e4.getAttribute("clienturlres");
e4.insertBefore(g8,null);
e4.posIndicator=g8;
return g8;
}
this.InitSpan=function (e4,e7,spans){
if (e7!=null){
var f5=0;
if (e7==this.GetViewport(e4))
f5=e7.rows.length;
var g9=e7.rows;
var h0=this.GetColCount(e4);
for (var h1=0;h1<g9.length;h1++){
if (this.IsChildSpreadRow(e4,e7,h1)){
if (e7==this.GetViewport(e4))f5--;
}else {
var h2=g9[h1].cells;
for (var h3=0;h3<h2.length;h3++){
var h4=h2[h3];
if (h4!=null&&((h4.rowSpan!=null&&h4.rowSpan>1)||(h4.colSpan!=null&&h4.colSpan>1))){
var h5=this.GetRowFromCell(e4,h4);
var h6=parseInt(h4.getAttribute("scol"));
if (h6<h0){
this.AddSpan(e4,h5,h6,h4.rowSpan,h4.colSpan,spans);
}
}
}
}
}
if (e7==this.GetViewport(e4))e4.setAttribute("rowCount",f5);
}
}
this.GetColWithSpan=function (e4,h1,spans,h3){
var h7=0;
var h8=0;
if (h3==0){
while (this.IsCovered(e4,h1,h8,spans))
{
h8++;
}
}
for (var f1=0;f1<spans.length;f1++){
if (spans[f1].rowCount>1&&(spans[f1].col<=h3||h3==0&&spans[f1].col<h8)&&h1>=spans[f1].row&&h1<spans[f1].row+spans[f1].rowCount)
h7+=spans[f1].colCount;
}
return h7;
}
this.AddSpan=function (e4,h1,h3,rc,h0,spans){
if (spans==null)spans=e4.d9;
var h9=new this.Range();
this.SetRange(h9,"Cell",h1,h3,rc,h0);
spans.push(h9);
this.PaintFocusRect(e4);
}
this.RemoveSpan=function (e4,h1,h3,spans){
if (spans==null)spans=e4.d9;
for (var f1=0;f1<spans.length;f1++){
var h9=spans[f1];
if (h9.row==h1&&h9.col==h3){
var i0=spans.length-1;
for (var i1=f1;i1<i0;i1++){
spans[i1]=spans[i1+1];
}
spans.length=spans.length-1;
break ;
}
}
this.PaintFocusRect(e4);
}
this.Focus=function (e4){
if (this.a7)return ;
this.SetPageActiveSpread(e4);
var i2=this.GetOperationMode(e4);
if (e4.d1==null&&i2!="MultiSelect"&&i2!="ExtendedSelect"&&e4.GetRowCount()>0&&e4.GetColCount()>0){
var i3=this.FireActiveCellChangingEvent(e4,0,0,0);
if (!i3){
e4.SetActiveCell(0,0);
var g3=this.CreateEvent("ActiveCellChanged");
g3.cmdID=e4.id;
g3.row=g3.Row=0;
g3.col=g3.Col=0;
if (e4.getAttribute("LayoutMode"))
g3.InnerRow=g3.innerRow=0;
this.FireEvent(e4,g3);
}
}
var e9=this.GetTopSpread(e4);
var g1=document.getElementById(e9.id+"_textBox");
if (e4.d1!=null){
var i4=this.GetEditor(e4.d1);
if (i4==null){
if (g1!=null){
if (this.b8!=g1){
try {g1.focus();g1.value="";}catch (g3){}
}
}
}else {
if (i4.tagName!="SELECT")i4.focus();
this.SetEditorFocus(i4);
}
}else {
if (g1!=null){
try {g1.focus();g1.value="";}catch (g3){}
}
}
this.EnableButtons(e4);
}
this.GetTotalRowCount=function (e4){
var g0=parseInt(e4.getAttribute("totalRowCount"));
if (isNaN(g0))g0=0;
return g0;
}
this.GetPageCount=function (e4){
var g0=parseInt(e4.getAttribute("pageCount"));
if (isNaN(g0))g0=0;
return g0;
}
this.GetColCount=function (e4){
var g0=parseInt(e4.getAttribute("colCount"));
if (isNaN(g0))g0=0;
return e4.frzCols+g0;
}
this.GetRowCount=function (e4){
var g0=parseInt(e4.getAttribute("rowCount"));
if (isNaN(g0))g0=0;
return g0;
}
this.GetRowCountInternal=function (e4){
var g0=parseInt(this.GetViewport(e4).rows.length);
if (isNaN(g0))g0=0;
return e4.frzRows+g0;
}
this.IsChildSpreadRow=function (e4,view,h1){
if (e4==null||view==null)return false;
if (h1>=1&&h1<view.rows.length){
var i5=view.rows[h1].getAttribute("isCSR");
if (i5!=null){
if (i5=="true")
return true;
else 
return false;
}
if (view.rows[h1].cells.length>0&&view.rows[h1].cells[0]!=null&&view.rows[h1].cells[0].firstChild!=null){
var g0=view.rows[h1].cells[0].firstChild;
if (g0.nodeName!="#text"&&g0.getAttribute("FpSpread")=="Spread"){
view.rows[h1].setAttribute("isCSR","true");
return true;
}
}
view.rows[h1].setAttribute("isCSR","false");
}
return false;
}
this.GetChildSpread=function (e4,row,rindex){
var i6=this.GetViewport(e4);
if (i6!=null){
var h1=this.GetDisplayIndex(e4,row)+1;
if (typeof(rindex)=="number")h1+=rindex;
if (h1>=1&&h1<i6.rows.length){
if (i6.rows[h1].cells.length>0&&i6.rows[h1].cells[0]!=null&&i6.rows[h1].cells[0].firstChild!=null){
var g0=i6.rows[h1].cells[0].firstChild;
if (g0.nodeName!="#text"&&g0.getAttribute("FpSpread")=="Spread"){
return g0;
}
}
}
}
return null;
}
this.GetChildSpreads=function (e4){
var f1=0;
var g5=new Array();
var i6=this.GetViewport(e4);
if (i6!=null){
for (var h1=1;h1<i6.rows.length;h1++){
if (i6.rows[h1].cells.length>0&&i6.rows[h1].cells[0]!=null&&i6.rows[h1].cells[0].firstChild!=null){
var g0=i6.rows[h1].cells[0].firstChild;
if (g0.nodeName!="#text"&&g0.getAttribute("FpSpread")=="Spread"){
g5.length=f1+1;
g5[f1]=g0;
f1++;
}
}
}
}
return g5;
}
this.GetDisplayIndex=function (e4,row){
if (row<0)return -1;
var f1=0;
var h1=0;
var i7=this.GetViewport0(e4);
if (i7==null)i7=this.GetViewport1(e4);
if (i7!=null){
if (row<i7.rows.length){
return row;
}
h1=i7.rows.length;
}
var i6=this.GetViewport(e4);
if (i6!=null){
for (f1=0;f1<i6.rows.length;f1++){
if (this.IsChildSpreadRow(e4,i6,f1))continue ;
if (h1==row)break ;
h1++;
}
}
if (i7!=null)f1+=i7.rows.length;
return f1;
}
this.GetSheetIndex=function (e4,row,c2){
var f1=0
var h1=0;
var i6=c2;
if (i6==null)i6=this.GetViewport(e4);
if (i6!=null){
if (row<0||row>=e4.frzRows+i6.rows.length)return -1;
for (f1=0;f1<row;f1++){
if (this.IsChildSpreadRow(e4,i6,f1))continue ;
h1++;
}
}
return h1;
}
this.GetParentRowIndex=function (e4){
var i8=this.GetParentSpread(e4);
if (i8==null)return -1;
var i6=this.GetViewport(i8);
if (i6==null)return -1;
var i9=e4.parentNode.parentNode;
var f1=i9.rowIndex-1;
for (;f1>0;f1--){
if (this.IsChildSpreadRow(i8,i6,f1))continue ;
else 
break ;
}
return this.GetSheetIndex(i8,f1,i6);
}
this.CreateTestBox=function (e4){
var j0=document.getElementById(e4.id+"_testBox");
if (j0==null)
{
j0=document.createElement("span");
j0.style.position="absolute";
j0.style.borderWidth=0;
j0.style.top="-500px";
j0.style.left="-100px";
j0.id=e4.id+"_testBox";
e4.insertBefore(j0,e4.firstChild);
}
return j0;
}
this.SizeToFit=function (e4,h3){
if (h3==null||h3<0)h3=0;
var e7=this.GetViewport(e4);
if (e7!=null){
var j0=this.CreateTestBox(e4);
var g9=e7.rows;
var j1=0;
for (var h1=0;h1<g9.length;h1++){
if (!this.IsChildSpreadRow(e4,e7,h1)){
var j2=this.GetCellFromRowCol(e4,h1,h3);
if (j2.colSpan>1)continue ;
var j3=this.GetPreferredCellWidth(e4,j2,j0);
if (j3>j1)j1=j3;
}
}
this.SetColWidth(e4,h3,j1);
}
}
this.GetPreferredCellWidth=function (e4,j2,j0){
if (j0==null)j0=this.CreateTestBox(e4);
var j4=this.GetRender(e4,j2);
var j5=this.GetCellType(j2);
var j6=this.GetEditor(j2);
if (j4!=null){
j0.style.fontFamily=j4.style.fontFamily;
j0.style.fontSize=j4.style.fontSize;
j0.style.fontWeight=j4.style.fontWeight;
j0.style.fontStyle=j4.style.fontStyle;
}
if (j4!=null&&j5=="MultiColumnComboBoxCellType"){
var j7=j2.getElementsByTagName("Table")[0];
if (j7!=null){
j0.innerHTML=this.GetEditorValue(j6)+"1";
}
}
else {
j0.innerHTML=j2.innerHTML;
}
var j3=j0.offsetWidth+8;
if (j2.style.paddingLeft!=null&&j2.style.paddingLeft.length>0)
j3+=parseInt(j2.style.paddingLeft);
if (j2.style.paddingRight!=null&&j2.style.paddingRight.length>0)
j3+=parseInt(j2.style.paddingRight);
return j3;
}
this.GetHierBar=function (e4){
if (e4.c8==null)e4.c8=document.getElementById(e4.id+"_hierBar");
return e4.c8;
}
this.GetGroupBar=function (e4){
if (e4.groupBar==null)e4.groupBar=document.getElementById(e4.id+"_groupBar");
return e4.groupBar;
}
this.GetPager1=function (e4){
if (e4.c9==null)e4.c9=document.getElementById(e4.id+"_pager1");
return e4.c9;
}
this.GetPager2=function (e4){
if (e4.d0==null)e4.d0=document.getElementById(e4.id+"_pager2");
return e4.d0;
}
this.SynRowHeight=function (e4,c5,e7,h1,updateParent,header){
if (c5==null||e7==null)return ;
var j8=c5.rows[h1].offsetHeight;
var g6=e7.rows[h1].offsetHeight;
if (j8==g6&&h1>0)return ;
var j9=this.IsXHTML(e4);
if (h1==0&&!j9){
j8+=c5.rows[h1].offsetTop;
g6+=e7.rows[h1].offsetTop;
}
if (j9)e7.rows[h1].style.height="";
var k0=Math.max(j8,g6);
if (c5.rows[h1].style.height=="")c5.rows[h1].style.height=""+j8+"px";
if (e7.rows[h1].style.height=="")e7.rows[h1].style.height=""+g6+"px";
if (this.IsChildSpreadRow(e4,e7,h1)){
c5.rows[h1].style.height=k0;
return ;
}
if (k0>0){
if (j9){
if (k0==j8)
e7.rows[h1].style.height=""+(parseInt(e7.rows[h1].style.height)+(k0-g6))+"px";
else 
c5.rows[h1].style.height=""+(parseInt(c5.rows[h1].style.height)+(k0-j8))+"px";
}else {
if (header&&e7.rows.length>=2&&e7.cellSpacing=="0"){
if (h1==0)
if (k0==j8)
e7.rows[h1].style.height=""+(parseInt(e7.rows[h1].style.height)+(k0-g6))+"px";
else 
c5.rows[h1].style.height=""+(parseInt(c5.rows[h1].style.height)+(k0-j8))+"px";
else 
{
if (e4.frzRows>0&&h1==e4.frzRows-1&&c5==this.GetFrozRowHeader(e4)){
j8+=this.GetRowHeader(e4).rows[0].offsetTop;
g6+=this.GetViewport(e4).rows[0].offsetTop;
c5.rows[h1].style.height=""+(parseInt(c5.rows[h1].style.height)+(Math.max(j8,g6)-j8))+"px";
}else {
c5.rows[h1].style.height=""+k0+"px";
e7.rows[h1].style.height=""+k0+"px";
}
}
}else {
if (k0==j8)
e7.rows[h1].style.height=""+(parseInt(e7.rows[h1].style.height)+(k0-g6))+"px";
else 
c5.rows[h1].style.height=""+(parseInt(c5.rows[h1].style.height)+(k0-j8))+"px";
}
}
}
if (updateParent){
var i8=this.GetParentSpread(e4);
if (i8!=null)this.UpdateRowHeight(i8,e4);
}
}
this.SizeAll=function (e4){
var k1=this.GetChildSpreads(e4);
if (k1!=null&&k1.length>0){
for (var f1=0;f1<k1.length;f1++){
this.SizeAll(k1[f1]);
}
}
this.SizeSpread(e4);
if (this.GetParentSpread(e4)!=null)
this.Refresh(e4);
}
this.EnsureAllRowHeights=function (e4){
if (this.GetFrozColHeader(e4)!=null&&this.GetColHeader(e4)!=null){
for (var f1=0;f1<this.GetFrozColHeader(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetFrozColHeader(e4),this.GetColHeader(e4),f1,false,false);
}
}
if (this.GetFrozColFooter(e4)!=null&&this.GetColFooter(e4)!=null){
for (var f1=0;f1<this.GetFrozColFooter(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetFrozColFooter(e4),this.GetColFooter(e4),f1,false,false);
}
}
if (this.GetViewport0(e4)!=null&&this.GetViewport1(e4)!=null){
for (var f1=0;f1<this.GetViewport1(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetViewport0(e4),this.GetViewport1(e4),f1,false,false);
this.SynRowHeight(e4,this.GetViewport0(e4),this.GetViewport1(e4),f1,false,false);
}
}
if (this.GetViewport(e4)!=null&&this.GetViewport2(e4)!=null){
for (var f1=0;f1<this.GetViewport(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetViewport2(e4),this.GetViewport(e4),f1,false,false);
this.SynRowHeight(e4,this.GetViewport2(e4),this.GetViewport(e4),f1,false,false);
}
}
if (this.GetFrozRowHeader(e4)!=null){
for (var f1=0;f1<this.GetViewport1(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetFrozRowHeader(e4),this.GetViewport1(e4),f1,false,true);
this.SynRowHeight(e4,this.GetFrozRowHeader(e4),this.GetViewport0(e4),f1,false,true);
}
}
if (this.GetRowHeader(e4)!=null){
for (var f1=0;f1<this.GetViewport(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetRowHeader(e4),this.GetViewport(e4),f1,false,true);
this.SynRowHeight(e4,this.GetRowHeader(e4),this.GetViewport2(e4),f1,false,true);
}
}
if (this.GetFrozColHeader(e4)!=null&&this.GetColHeader(e4)!=null){
for (var f1=0;f1<this.GetFrozColHeader(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetFrozColHeader(e4),this.GetColHeader(e4),f1,false,false);
}
}
if (this.GetViewport0(e4)!=null&&this.GetViewport1(e4)!=null){
for (var f1=0;f1<this.GetViewport1(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetViewport0(e4),this.GetViewport1(e4),f1,false,false);
this.SynRowHeight(e4,this.GetViewport0(e4),this.GetViewport1(e4),f1,false,false);
}
}
if (this.GetViewport(e4)!=null&&this.GetViewport2(e4)!=null){
for (var f1=0;f1<this.GetViewport(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetViewport2(e4),this.GetViewport(e4),f1,false,false);
this.SynRowHeight(e4,this.GetViewport2(e4),this.GetViewport(e4),f1,false,false);
}
}
if (this.GetFrozRowHeader(e4)!=null){
for (var f1=0;f1<this.GetViewport1(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetFrozRowHeader(e4),this.GetViewport1(e4),f1,false,true);
this.SynRowHeight(e4,this.GetFrozRowHeader(e4),this.GetViewport0(e4),f1,false,true);
}
}
if (this.GetRowHeader(e4)!=null){
for (var f1=0;f1<this.GetViewport(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetRowHeader(e4),this.GetViewport(e4),f1,false,true);
this.SynRowHeight(e4,this.GetRowHeader(e4),this.GetViewport2(e4),f1,false,true);
}
}
if (this.GetCorner(e4)!=null){
if (this.GetCorner(e4).getAttribute("allowTableCorner")!=null){
if (this.GetCorner(e4)!=null&&this.GetColHeader(e4)!=null){
for (var f1=0;f1<this.GetCorner(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetCorner(e4),this.GetColHeader(e4),f1,false,false);
}
}
if (this.GetFrozColHeader(e4)!=null&&this.GetCorner(e4)!=null){
for (var f1=0;f1<this.GetCorner(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetCorner(e4),this.GetFrozColHeader(e4),f1,false,false);
}
}
if (this.GetCorner(e4)!=null&&this.GetColHeader(e4)!=null){
for (var f1=0;f1<this.GetCorner(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetCorner(e4),this.GetColHeader(e4),f1,false,false);
}
}
if (this.GetFrozColHeader(e4)!=null&&this.GetCorner(e4)!=null){
for (var f1=0;f1<this.GetCorner(e4).rows.length;f1++){
this.SynRowHeight(e4,this.GetCorner(e4),this.GetFrozColHeader(e4),f1,false,false);
}
}
}
}
}
this.SizeSpread=function (e4,skipRowHeight){
var j9=this.IsXHTML(e4);
var c2=this.GetViewport(e4);
if (c2==null)return ;
if (skipRowHeight==null)this.EnsureAllRowHeights(e4);
var c6=this.GetColHeader(e4);
var k2=this.GetColGroup(c2);
var k3=this.GetColGroup(c6);
if (k2!=null&&k2.childNodes.length>0&&k3!=null&&k3.childNodes.length>0){
var k4=-1;
if (this.b4!=null)k4=parseInt(this.b4.getAttribute("index"));
if ((this.b4==null||k4==0)&&e4.frzCols>=0)
{
k3.childNodes[0].width=(k2.childNodes[0].offsetLeft+k2.childNodes[0].offsetWidth-c2.cellSpacing);
}
}
var k5=this.GetFrozColHeader(e4);
if (k2!=null&&k2.childNodes.length>0&&k5!=null){
var k6=0;
var k7=this.GetColGroup(this.GetViewport2(e4));
for (var f1=0;f1<k7.childNodes.length;f1++)k6+=k7.childNodes[f1].offsetWidth;
k5.parentNode.parentNode.style.width=""+(k6+k2.childNodes[0].offsetLeft)+"Px";
}
this.SyncMsgs(e4);
if (e4.frzCols>0){
var k8=this.GetFrozColHeader(e4);
if (k8!=null){
var k9=parseInt(k8.parentNode.parentNode.style.width);
if (k8!=null)
k8.parentNode.style.width=""+k9+"px";
var l0=this.GetFrozColFooter(e4);
if (l0!=null)
l0.parentNode.style.width=""+k9+"px";
}
}
if (skipRowHeight==null)this.EnsureAllRowHeights(e4);
var c5=this.GetRowHeader(e4);
var c6=this.GetColHeader(e4);
var l1=this.GetColFooter(e4);
var i8=this.GetParentSpread(e4);
if (i8!=null)this.UpdateRowHeight(i8,e4);
var k0=e4.clientHeight;
var l2=this.GetCommandBar(e4);
if (l2!=null)
{
l2.style.width=""+e4.clientWidth+"px";
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
l2.parentNode.style.borderTop="1px solid white";
l2.parentNode.style.backgroundColor=l2.style.backgroundColor;
}
var l3=this.GetElementById(l2,e4.id+"_cmdTable");
if (l3!=null){
if (e4.style.position!="absolute"&&e4.style.position!="relative"&&(l3.style.height==""||parseInt(l3.style.height)<27)){
l3.style.height=""+(l3.offsetHeight+3)+"px";
}
if (!j9&&parseInt(c2.cellSpacing)>0)
l3.parentNode.style.height=""+(l3.offsetHeight+3)+"px";
k0-=Math.max(l3.parentNode.offsetHeight,l3.offsetHeight);
}
if (l3.offsetHeight>l3.parentNode.offsetHeight)k0+=2;
if (e4.style.position!="absolute"&&e4.style.position!="relative")
l2.style.position="";
}
var c6=this.GetColHeader(e4);
if (c6!=null)
{
if (!e4.initialized)
c6.parentNode.style.height=""+(c6.offsetHeight-parseInt(c6.cellSpacing))+"px";
k0-=c6.parentNode.offsetHeight;
if (j9)
k0+=parseInt(c6.cellSpacing);
}
var l1=this.GetColFooter(e4);
if (l1!=null)
{
k0-=l1.offsetHeight;
l1.parentNode.style.height=""+(l1.offsetHeight)+"px";
}
var c8=this.GetHierBar(e4);
if (c8!=null)
{
k0-=c8.offsetHeight;
}
var l4=this.GetGroupBar(e4);
if (l4!=null){
k0-=l4.offsetHeight;
}
var c9=this.GetPager1(e4);
if (c9!=null)
{
k0-=c9.offsetHeight;
this.InitSlideBar(e4,c9);
}
if (!j9&&e4.frzRows>0&&c5){
var j8=c5.rows[0].offsetTop;
var g6=this.GetViewport(e4).rows[0].offsetTop;
k0-=(g6-j8);
}
var l5=(e4.getAttribute("cmdTop")=="true");
var d0=this.GetPager2(e4);
if (d0!=null)
{
d0.style.width=""+(e4.clientWidth-10)+"px";
k0-=Math.max(d0.offsetHeight,28);
this.InitSlideBar(e4,d0);
}
var l6=null;
if (c5!=null)l6=c5.parentNode;
var l7=null;
if (c6!=null)l7=c6.parentNode;
var l8=null;
if (l1!=null)l8=l1.parentNode;
var l9=this.GetFooterCorner(e4);
if (l8!=null)
{
l8.style.height=""+l1.offsetHeight-parseInt(c2.cellSpacing)+"px";
if (l9!=null){
l9.parentNode.style.height=l8.style.height;
}
}
if (l9!=null&&!j9)
l9.width=""+(l9.parentNode.offsetWidth+parseInt(c2.cellSpacing))+"px";
var c4=this.GetCorner(e4);
if (l7!=null)
{
if (!e4.initialized)
l7.style.height=""+c6.offsetHeight-parseInt(c2.cellSpacing)+"px";
if (c4!=null){
c4.parentNode.style.height=l7.style.height;
}
}
if (c4!=null&&!j9)
c4.width=""+(c4.parentNode.offsetWidth+parseInt(c2.cellSpacing))+"px";
var m0=0;
if (this.GetColFooter(e4)){
m0=this.GetColFooter(e4).offsetHeight;
}
if (l2!=null&&!l5){
if (d0!=null){
if (e4.style.position=="absolute"||e4.style.position=="relative"){
l2.style.position="absolute";
l2.style.top=""+(e4.clientHeight-Math.max(d0.offsetHeight,28)-l2.offsetHeight)+"px";
}else {
l2.style.position="absolute";
l2.style.top=""+(c2.parentNode.offsetTop+m0+c2.parentNode.offsetHeight)+"px";
}
}else {
if (e4.style.position=="absolute"||e4.style.position=="relative"){
l2.style.position="absolute";
l2.style.top=""+(e4.clientHeight-l2.offsetHeight)+"px";
}else {
l2.style.position="absolute";
if (d0!=null)
l2.style.top=""+(this.GetOffsetTop(e4,e4,document.body)+e4.clientHeight-Math.max(d0.offsetHeight,28)-l2.offsetHeight)+"px";
else 
l2.style.top=""+(this.GetOffsetTop(e4,e4,document.body)+e4.clientHeight-l2.offsetHeight+1)+"px";
}
}
}
if (d0!=null)
{
if (e4.style.position=="absolute"||e4.style.position=="relative"){
d0.style.position="absolute";
d0.style.top=""+(e4.clientHeight-Math.max(d0.offsetHeight,28))+"px";
}else {
d0.style.position="absolute";
if (l2!=null&&!l5)
d0.style.top=""+(c2.parentNode.offsetTop+c2.parentNode.offsetHeight+l2.offsetHeight+m0)+"px";
else 
d0.style.top=""+(c2.parentNode.offsetTop+c2.parentNode.offsetHeight+m0)+"px";
}
}
var m1=this.GetViewport0(e4);
var m2=this.GetViewport1(e4);
var m3=this.GetViewport2(e4);
if (m2!=null){
m2.parentNode.style.height=""+Math.max(0,m2.offsetHeight-m2.cellSpacing)+"px";
if (m1!=null){
m1.parentNode.style.height=m2.parentNode.style.height;
m1.parentNode.style.width=""+(m1.offsetWidth-m1.cellSpacing)+"px"
}
}
if (m3!=null){
m3.parentNode.style.width=""+(m3.offsetWidth-m3.cellSpacing)+"px"
}
var m4=this.GetFrozRowHeader(e4);
if (m4!=null){
var m5=m4.offsetHeight-m4.cellSpacing;
m4.parentNode.style.height=""+Math.max(0,m5)+"px";
}
var m6=e4.clientWidth;
if (c5!=null)m6-=c5.parentNode.offsetWidth;
if (m3!=null)m6-=m3.offsetWidth;
else if (m1!=null)m6-=m1.offsetWidth;
if (m1!=null)k0-=m1.offsetHeight;
else if (m2!=null)k0-=m2.offsetHeight;
if (e4.frzRows>0)k0+=parseInt(c2.cellSpacing);
if (!j9)k0+=parseInt(c2.cellSpacing);
var m7=document.getElementById(e4.id+"_titleBar");
if (m7)k0-=m7.parentNode.parentNode.offsetHeight;
c2.parentNode.style.height=""+Math.max(k0,1)+"px";
if (e4.frzCols>0)m6+=parseInt(c2.cellSpacing);
c2.parentNode.style.width=""+(m6+parseInt(c2.cellSpacing))+"px";
if (m2!=null){
m2.parentNode.style.width=""+(c2.parentNode.clientWidth)+"px";
}
if (m3!=null)m3.parentNode.style.height=""+(c2.parentNode.clientHeight)+"px";
if (l6!=null){
l6.style.height=""+Math.max(c2.parentNode.offsetHeight,1)+"px";
}
if (this.GetParentSpread(e4)==null&&l7!=null&&l6!=null&&e4.frzCols==0){
var j3=0;
if (l6!=null){
j3=Math.max(e4.clientWidth-l6.offsetWidth,1);
}else {
j3=Math.max(e4.clientWidth,1);
}
l7.style.width=j3;
l7.parentNode.style.width=j3;
}
this.ScrollView(e4);
this.PaintFocusRect(e4);
if (c2&&!c5&&!m1&&!m2&&!m3){
c2.parentNode.parentNode.parentNode.style.height=""+c2.parentNode.offsetHeight+"px";
}
}
this.InitSlideBar=function (e4,pager){
var m8=this.GetElementById(pager,e4.id+"_slideBar");
if (m8!=null){
var j9=this.IsXHTML(e4);
if (j9)
m8.style.height=Math.max(pager.offsetHeight,28)+"px";
else 
m8.style.height=(pager.offsetHeight-2)+"px";
var g0=pager.getElementsByTagName("TABLE");
if (g0!=null&&g0.length>0){
var m9=g0[0].rows[0];
var h6=m9.cells[0];
var n0=m9.cells[2];
e4.slideLeft=Math.max(107,h6.offsetWidth+1);
if (h6.style.paddingRight!="")e4.slideLeft+=parseInt(h6.style.paddingRight);
e4.slideRight=pager.offsetWidth-n0.offsetWidth-m8.offsetWidth-3;
if (n0.style.paddingRight!="")e4.slideRight-=parseInt(n0.style.paddingLeft);
var n1=parseInt(pager.getAttribute("curPage"));
var n2=parseInt(pager.getAttribute("totalPage"))-1;
if (n2==0)n2=1;
var n3=false;
var m6=Math.max(107,e4.slideLeft)+(n1/n2)*(e4.slideRight-e4.slideLeft);
if (pager.id.indexOf("pager1")>=0&&e4.style.position!="absolute"&&e4.style.position!="relative"){
m6+=this.GetOffsetLeft(e4,pager,document);
var n4=(this.GetOffsetTop(e4,h6,pager)+this.GetOffsetTop(e4,pager,document));
m8.style.top=n4+"px";
n3=true;
}
var m7=document.getElementById(e4.id+"_titleBar");
if (pager.id.indexOf("pager1")>=0&&!n3&&m7!=null){
var n4=m7.parentNode.parentNode.offsetHeight;
m8.style.top=n4+"px";
}
m8.style.left=m6+"px";
}
}
}
this.InitLayout=function (e4){
this.SizeSpread(e4);
this.SizeSpread(e4);
}
this.GetRowByKey=function (e4,key){
if (key=="-1")
return -1;
var n5=this.GetViewport1(e4);
if (n5!=null){
var n6=n5.rows.length;
var g9=n5.rows;
for (var i9=0;i9<n6;i9++){
if (g9[i9].getAttribute("FpKey")==key){
return i9;
}
}
}
var n7=this.GetViewport(e4);
if (n7!=null){
var n6=n7.rows.length;
var g9=n7.rows;
for (var i9=0;i9<n6;i9++){
if (g9[i9].getAttribute("FpKey")==key){
if (n5!=null)i9+=n5.rows.length;
return i9;
}
}
}
if (n7!=null)
return 0;
else 
return -1;
}
this.GetColByKey=function (e4,key){
if (key=="-1")
return -1;
var n8=null;
var n5=this.GetViewport0(e4);
if (n5==null||n5.rows.length==0)n5=this.GetViewport2(e4);
if (n5!=null){
n8=this.GetColGroup(n5);
if (n8!=null){
for (var n9=0;n9<n8.childNodes.length;n9++){
var g0=n8.childNodes[n9];
if (g0.getAttribute("FpCol")==key){
return n9;
}
}
}
}
var n7=this.GetViewport(e4);
var f3=this.GetColGroup(n7);
if (f3==null||f3.childNodes.length==0)
f3=this.GetColGroup(this.GetColHeader(e4));
if (f3!=null){
for (var n9=0;n9<f3.childNodes.length;n9++){
var g0=f3.childNodes[n9];
if (g0.getAttribute("FpCol")==key){
if (n8!=null){
n9+=n8.childNodes.length;
}
return n9;
}
}
}
return 0;
}
this.IsRowSelected=function (e4,i9){
var o0=this.GetSelection(e4);
if (o0!=null){
var o1=o0.firstChild;
while (o1!=null){
var h1;
var n6;
var o2=this.GetOperationMode(e4);
if (e4.getAttribute("LayoutMode")&&(o2=="ExtendedSelect"||o2=="MultiSelect")){
var o3=parseInt(o1.getAttribute("row"));
h1=this.GetFirstRowFromKey(e4,o3);
}
else 
h1=parseInt(o1.getAttribute("rowIndex"));
if (e4.getAttribute("LayoutMode")&&(o2=="ExtendedSelect"||o2=="MultiSelect"))
n6=parseInt(e4.getAttribute("layoutrowcount"));
else 
n6=parseInt(o1.getAttribute("rowcount"));
if (h1<=i9&&i9<h1+n6)
return true;
o1=o1.nextSibling;
}
}
}
this.InitSelection=function (e4){
var h1=0;
var h3=0;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var o0=o4.getElementsByTagName("selection")[0];
var o5=o4.firstChild;
while (o5!=null&&o5.tagName!="activerow"&&o5.tagName!="ACTIVEROW"){
o5=o5.nextSibling;
}
if (o5!=null&&!e4.getAttribute("LayoutMode"))
h1=this.GetRowByKey(e4,o5.innerHTML);
if (h1>=this.GetRowCount(e4))h1=0;
var o6=o4.firstChild;
while (o6!=null&&o6.tagName!="activecolumn"&&o6.tagName!="ACTIVECOLUMN"){
o6=o6.nextSibling;
}
if (o6!=null&&!e4.getAttribute("LayoutMode"))
h3=this.GetColByKey(e4,o6.innerHTML);
if (e4.getAttribute("LayoutMode")&&o5!=null&&o6!=null){
h1=parseInt(o5.innerHTML);
h3=parseInt(o6.innerHTML);
var h4;
if (h1!=-1&&h3!=-1)h4=this.GetCellByRowCol2(e4,o5.innerHTML,o6.innerHTML);
if (h4){
h1=this.GetRowFromCell(e4,h4);
h3=this.GetColFromCell(e4,h4);
}
}
if (h1<0)h1=0;
if (h1>=0||h3>=0){
var o7=f6;
if (this.GetParentSpread(e4)!=null){
var o8=this.GetTopSpread(e4);
if (o8.initialized)o7=this.GetData(o8);
f7=o7.getElementsByTagName("root")[0];
}
var o9=f7.getElementsByTagName("activechild")[0];
e4.d3=h1;e4.d4=h3;
if ((this.GetParentSpread(e4)==null&&(o9==null||o9.innerHTML==""))||(o9!=null&&e4.id==this.Trim(o9.innerHTML))){
this.UpdateAnchorCell(e4,h1,h3);
}else {
e4.d1=this.GetCellFromRowCol(e4,h1,h3);
}
}
var o1=o0.firstChild;
while (o1!=null){
var h1=0;
var h3=0;
if (e4.getAttribute("LayoutMode")&&o1.getAttribute("row")!="-1"&&o1.getAttribute("col")!="-1"){
var h4=this.GetCellByRowCol2(e4,o1.getAttribute("row"),o1.getAttribute("col"));
if (h4){
h1=this.GetRowFromCell(e4,h4);
h3=this.GetColFromCell(e4,h4);
}
}
else if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")!="-1"&&o1.getAttribute("row")=="-1"&&o1.getAttribute("rowcount")=="-1"){
var i9=this.GetRowTemplateRowFromGroupCell(e4,parseInt(o1.getAttribute("col")));
var h4=this.GetCellByRowCol2(e4,i9,parseInt(o1.getAttribute("col")));
if (h4){
h1=parseInt(h4.parentNode.getAttribute("row"));
h3=this.GetColFromCell(e4,h4);
}
}
else {
h1=this.GetRowByKey(e4,o1.getAttribute("row"));
h3=this.GetColByKey(e4,o1.getAttribute("col"));
}
var n6=parseInt(o1.getAttribute("rowcount"));
var h0=parseInt(o1.getAttribute("colcount"));
o1.setAttribute("rowIndex",h1);
o1.setAttribute("colIndex",h3);
if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")>=0&&o1.getAttribute("row")>=0&&(o1.getAttribute("rowcount")>=1||o1.getAttribute("colcount")>=1)){
var p0=o1.nextSibling;
if (parseInt(o1.getAttribute("row"))!=parseInt(o5.innerHTML)||parseInt(o1.getAttribute("col"))!=parseInt(o6.innerHTML))o0.removeChild(o1);
o1=p0;
continue ;
}
if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")=="-1"&&o1.getAttribute("row")!=-1){
n6=parseInt(e4.getAttribute("layoutrowcount"));
}
if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")!="-1"&&o1.getAttribute("row")=="-1"&&o1.getAttribute("rowcount")=="-1")
this.PaintMultipleRowSelection(e4,h1,h3,1,1,true);
else 
this.PaintSelection(e4,h1,h3,n6,h0,true);
o1=o1.nextSibling;
}
this.PaintFocusRect(e4);
}
this.TranslateKeyPress=function (event){
if (event.ctrlKey&&!event.altKey){
this.TranslateKey(event);
}
var e4=this.GetPageActiveSpread();
if (e4!=null&&!this.a7&&event.keyCode==event.DOM_VK_RETURN)this.CancelDefault(event);
}
this.TranslateKey=function (event){
event=this.GetEvent(event);
var p1=this.GetTarget(event);
try {
if (document.readyState!=null&&document.readyState!="complete")return ;
var e4=this.GetPageActiveSpread();
if (typeof(e4.getAttribute("mcctCellType"))!="undefined"&&e4.getAttribute("mcctCellType")=="true")return ;
if (this.GetOperationMode(e4)=="RowMode"&&this.GetEnableRowEditTemplate(e4)=="true"&&this.IsInRowEditTemplate(e4,p1))return ;
if (e4!=null){
if (event.keyCode==229){
this.CancelDefault(event);
return ;
}
if (p1.tagName!="HTML"&&!this.IsChild(p1,this.GetTopSpread(e4)))return ;
this.KeyDown(e4,event);
var p2=false;
if (event.keyCode==event.DOM_VK_TAB){
var p3=this.GetProcessTab(e4);
p2=(p3=="true"||p3=="True");
}
if (p2)
this.CancelDefault(event);
}
}catch (g3){}
}
this.IsInRowEditTemplate=function (e4,p1){
while (p1&&p1.parentNode){
p1=p1.parentNode;
if (p1.tagName=="DIV"&&p1.id==e4.id+"_RowEditTemplateContainer")
return true;
}
return false;
}
this.KeyAction=function (key,ctrl,shift,alt,action){
this.key=key;
this.ctrl=ctrl;
this.shift=shift;
this.alt=alt;
this.action=action;
}
this.RemoveKeyMap=function (e4,keyCode,ctrl,shift,alt,action){
if (e4.keyMap==null)e4.keyMap=new Array();
var f0=e4.keyMap.length;
for (var f1=0;f1<f0;f1++){
var g4=e4.keyMap[f1];
if (g4!=null&&g4.key==keyCode&&g4.ctrl==ctrl&&g4.shift==shift&&g4.alt==alt){
for (var i1=f1+1;i1<f0;i1++){
e4.keyMap[i1-1]=e4.keyMap[i1];
}
e4.keyMap.length=e4.keyMap.length-1;
break ;
}
}
}
this.AddKeyMap=function (e4,keyCode,ctrl,shift,alt,action){
if (e4.keyMap==null)e4.keyMap=new Array();
var g4=this.GetKeyAction(e4,keyCode,ctrl,shift,alt);
if (g4!=null){
g4.action=action;
}else {
var f0=e4.keyMap.length;
e4.keyMap.length=f0+1;
e4.keyMap[f0]=new this.KeyAction(keyCode,ctrl,shift,alt,action);
}
}
this.GetKeyAction=function (e4,keyCode,ctrl,shift,alt){
if (e4.keyMap==null)e4.keyMap=new Array();
var f0=e4.keyMap.length;
for (var f1=0;f1<f0;f1++){
var g4=e4.keyMap[f1];
if (g4!=null&&g4.key==keyCode&&g4.ctrl==ctrl&&g4.shift==shift&&g4.alt==alt){
return g4;
}
}
return null;
}
this.MoveToPrevCell=function (e4){
var p4=this.EndEdit(e4);
if (!p4)return ;
var h1=e4.GetActiveRow();
var h3=e4.GetActiveCol();
this.MoveLeft(e4,h1,h3);
}
this.MoveToNextCell=function (e4){
var p4=this.EndEdit(e4);
if (!p4)return ;
var h1=e4.GetActiveRow();
var h3=e4.GetActiveCol();
this.MoveRight(e4,h1,h3);
}
this.MoveToNextRow=function (e4){
var p4=this.EndEdit(e4);
if (!p4)return ;
var h1=e4.GetActiveRow();
var h3=e4.GetActiveCol();
this.MoveDown(e4,h1,h3);
}
this.MoveToPrevRow=function (e4){
var p4=this.EndEdit(e4);
if (!p4)return ;
var h1=e4.GetActiveRow();
var h3=e4.GetActiveCol();
if (h1>0)
this.MoveUp(e4,h1,h3);
}
this.MoveToFirstColumn=function (e4){
var p4=this.EndEdit(e4);
if (!p4)return ;
var h1=e4.GetActiveRow();
if (e4.d1.parentNode.rowIndex>=0)
this.UpdateLeadingCell(e4,h1,0);
}
this.MoveToLastColumn=function (e4){
var p4=this.EndEdit(e4);
if (!p4)return ;
var h1=e4.GetActiveRow();
if (e4.d1.parentNode.rowIndex>=0){
h3=this.GetColCount(e4)-1;
this.UpdateLeadingCell(e4,h1,h3);
}
}
this.UpdatePostbackData=function (e4){
this.SaveData(e4);
}
this.PrepareData=function (o1){
var g5="";
if (o1!=null){
if (o1.nodeName=="#text")
g5=o1.nodeValue;
else {
g5=this.GetBeginData(o1);
var g0=o1.firstChild;
while (g0!=null){
var p5=this.PrepareData(g0);
if (p5!="")g5+=p5;
g0=g0.nextSibling;
}
g5+=this.GetEndData(o1);
}
}
return g5;
}
this.GetBeginData=function (o1){
var g5="<"+o1.nodeName.toLowerCase();
if (o1.attributes!=null){
for (var f1=0;f1<o1.attributes.length;f1++){
var p6=o1.attributes[f1];
if (p6.nodeName!=null&&p6.nodeName!=""&&p6.nodeName!="style"&&p6.nodeValue!=null&&p6.nodeValue!="")
g5+=(" "+p6.nodeName+"=\""+p6.nodeValue+"\"");
}
}
g5+=">";
return g5;
}
this.GetEndData=function (o1){
return "</"+o1.nodeName.toLowerCase()+">";
}
this.SaveData=function (e4){
if (e4==null)return ;
try {
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var g0=this.PrepareData(f7);
var p7=document.getElementById(e4.id+"_data");
p7.value=encodeURIComponent(g0);
}catch (g3){
alert("e "+g3);
}
}
this.SetActiveSpread=function (event){
try {
event=this.GetEvent(event);
var p1=this.GetTarget(event);
var p8=this.GetSpread(p1,false);
var p9=this.GetPageActiveSpread();
if (this.a7&&(p8==null||(p8!=p9&&p8.getAttribute("mcctCellType")!="true"&&p9.getAttribute("mcctCellType")!="true"))){
if (p1!=this.a8&&this.a8!=null){
if (this.a8.blur!=null)this.a8.blur();
}
var p4=this.EndEdit();
if (!p4)return ;
}
var q0=false;
if (p8==null){
p8=this.GetSpread(p1,true);
q0=(p8!=null);
}
var h4=this.GetCell(p1,true);
if (h4==null&&p9!=null&&p9.e2){
this.SaveData(p9);
p9.e2=false;
}
if (p9!=null&&p9.e2&&(p8!=p9||p8==null||q0)){
this.SaveData(p9);
p9.e2=false;
}
if (p9!=null&&p9.e2&&p8==p9&&p1.tagName=="INPUT"&&(p1.type=="submit"||p1.type=="button"||p1.type=="image")){
this.SaveData(p9);
p9.e2=false;
}
if (p8!=null&&this.GetOperationMode(p8)=="ReadOnly")return ;
var o8=null;
if (p8==null){
if (p9==null)return ;
o8=this.GetTopSpread(p9);
this.SetActiveSpreadID(o8,"",null,false);
this.SetPageActiveSpread(null);
}else {
if (p8!=p9){
if (p9!=null){
o8=this.GetTopSpread(p9);
this.SetActiveSpreadID(o8,"",null,false);
}
if (q0){
o8=this.GetTopSpread(p8);
var q1=this.GetTopSpread(p9);
if (o8!=q1){
this.SetActiveSpreadID(o8,p8.id,p8.id,true);
this.SetPageActiveSpread(p8);
}else {
this.SetActiveSpreadID(o8,p9.id,p9.id,true);
this.SetPageActiveSpread(p9);
}
}else {
o8=this.GetTopSpread(p8);
this.SetPageActiveSpread(p8);
this.SetActiveSpreadID(o8,p8.id,p8.id,false);
}
}
}
}catch (g3){}
}
this.SetActiveSpreadID=function (e4,id,child,q0){
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var f8=f7.getElementsByTagName("activespread")[0];
var q2=f7.getElementsByTagName("activechild")[0];
if (f8==null)return ;
if (q0&&q2!=null&&q2.nodeValue!=""){
f8.innerHTML=q2.innerHTML;
}else {
f8.innerHTML=id;
if (child!=null&&q2!=null)q2.innerHTML=child;
}
this.SaveData(e4);
e4.e2=false;
}
this.GetSpread=function (ele,incCmdBar){
var j3=ele;
while (j3!=null&&j3.tagName!="BODY"){
if (typeof(j3.getAttribute)!="function")break ;
var e4=j3.getAttribute("FpSpread");
if (e4==null)e4=j3.FpSpread;
if (e4=="Spread"){
if (!incCmdBar){
var g0=ele;
while (g0!=null&&g0!=j3){
if (g0.id==j3.id+"_commandBar"||g0.id==j3.id+"_pager1"||g0.id==j3.id+"_pager2")return null;
g0=g0.parentNode;
}
}
return j3;
}
j3=j3.parentNode;
}
return null;
}
this.GetActiveChildSheetView=function (e4){
var p9=this.GetPageActiveSheetView();
if (typeof(p9)=="undefined")return null;
var o8=this.GetTopSpread(e4);
var q3=this.GetTopSpread(p9);
if (q3!=o8)return null;
if (p9==q3)return null;
return p9;
}
this.ScrollViewport=function (event){
var g0=this.GetTarget(event);
var e4=this.GetTopSpread(g0);
if (e4!=null)this.ScrollView(e4);
}
this.ScrollTo=function (e4,i9,n9){
var h4=this.GetCellByRowCol(e4,i9,n9);
if (h4==null)return ;
var i6=this.GetViewport(e4).parentNode;
if (i6==null)return ;
i6.scrollTop=h4.offsetTop;
i6.scrollLeft=h4.offsetLeft;
}
this.ScrollView=function (e4){
var p8=this.GetTopSpread(e4);
var c5=this.GetParent(this.GetRowHeader(p8));
var c6=this.GetParent(this.GetColHeader(p8));
var l1=this.GetParent(this.GetColFooter(p8));
var i6=this.GetParent(this.GetViewport(p8));
var q4=false;
if (c5!=null){
q4=(c5.scrollTop!=i6.scrollTop);
c5.scrollTop=i6.scrollTop;
}
if (c6!=null){
if (!q4)q4=(c6.scrollLeft!=i6.scrollLeft);
c6.scrollLeft=i6.scrollLeft;
}
if (l1!=null){
if (!q4)q4=(l1.scrollLeft!=i6.scrollLeft);
l1.scrollLeft=i6.scrollLeft;
}
var q5=this.GetViewport0(e4);
var q6=this.GetViewport1(e4);
var q7=this.GetViewport2(e4);
if (q7!=null){
q7.parentNode.scrollTop=i6.scrollTop;
}
if (q6!=null){
q6.parentNode.scrollLeft=i6.scrollLeft;
}
if (this.GetParentSpread(e4)==null)this.SaveScrollbarState(e4,i6.scrollTop,i6.scrollLeft);
if (q4){
var g3=this.CreateEvent("Scroll");
this.FireEvent(e4,g3);
if (e4.frzRows!=0||e4.frzCols!=0)this.SyncMsgs(e4);
}
if (i6.scrollTop>0&&i6.scrollTop+i6.offsetHeight>=this.GetViewport(p8).offsetHeight){
if (e4.initialized&&!this.a7&&e4.getAttribute("loadOnDemand")=="true"){
if (e4.LoadState!=null)return ;
e4.LoadState=true;
this.SaveData(e4);
setTimeout(e4.CallBack("LoadOnDemand",true),0);
}
}
}
this.SaveScrollbarState=function (e4,scrollTop,scrollLeft){
if (this.GetParentSpread(e4)!=null)return ;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var q8=f7.getElementsByTagName("scrollTop")[0];
var q9=f7.getElementsByTagName("scrollLeft")[0];
if (e4.getAttribute("scrollContent"))
if (q8!=null&&q9!=null)
if (q8.innerHTML!=scrollTop||q9.innerHTML!=scrollLeft)
this.ShowScrollingContent(e4,q8.innerHTML==scrollTop);
if (q8!=null)q8.innerHTML=scrollTop;
if (q9!=null)q9.innerHTML=scrollLeft;
}
this.LoadScrollbarState=function (e4){
if (this.GetParentSpread(e4)!=null)return ;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var q8=f7.getElementsByTagName("scrollTop")[0];
var q9=f7.getElementsByTagName("scrollLeft")[0];
var r0=0;
if (q8!=null&&q8.innerHTML!=""){
r0=parseInt(q8.innerHTML);
}else {
r0=0;
}
var r1=0;
if (q9!=null&&q9.innerHTML!=""){
r1=parseInt(q9.innerHTML);
}else {
r1=0;
}
var i6=this.GetParent(this.GetViewport(e4));
if (i6!=null){
if (!isNaN(r0))i6.scrollTop=r0;
if (!isNaN(r1))i6.scrollLeft=r1;
var c5=this.GetParent(this.GetRowHeader(e4));
var c6=this.GetParent(this.GetColHeader(e4));
var l1=this.GetParent(this.GetColFooter(e4));
if (l1!=null){
l1.scrollLeft=i6.scrollLeft;
}
if (c5!=null){
c5.scrollTop=i6.scrollTop;
}
if (c6!=null){
c6.scrollLeft=i6.scrollLeft;
}
}
}
this.GetParent=function (g3){
if (g3==null)
return null;
else 
return g3.parentNode;
}
this.GetViewport=function (e4){
return e4.c2;
}
this.GetFrozColHeader=function (e4){
return e4.frozColHeader;
}
this.GetColFooter=function (e4){
return e4.colFooter;
}
this.GetFrozColFooter=function (e4){
return e4.frozColFooter;
}
this.GetTopTable=function (e4){
return e4.getElementsByTagName("TABLE")[0];
}
this.GetFrozRowHeader=function (e4){
return e4.frozRowHeader;
}
this.GetViewport0=function (e4){
return e4.viewport0;
}
this.GetViewport1=function (e4){
return e4.viewport1;
}
this.GetViewport2=function (e4){
return e4.viewport2;
}
this.GetCommandBar=function (e4){
return e4.c7;
}
this.GetRowHeader=function (e4){
return e4.c5;
}
this.GetColHeader=function (e4){
return e4.c6;
}
this.GetCmdBtn=function (e4,id){
var p8=this.GetTopSpread(e4);
var r2=this.GetCommandBar(p8);
if (r2!=null)
return this.GetElementById(r2,p8.id+"_"+id);
else 
return null;
}
this.Range=function (){
this.type="Cell";
this.row=-1;
this.col=-1;
this.rowCount=0;
this.colCount=0;
this.innerRow=0;
}
this.SetRange=function (h9,type,i9,n9,n6,h0,innerRow){
h9.type=type;
h9.row=i9;
h9.col=n9;
h9.rowCount=n6;
h9.colCount=h0;
h9.innerRow=innerRow;
if (type=="Row"){
h9.col=h9.colCount=-1;
}else if (type=="Column"){
h9.row=h9.rowCount=-1;
}else if (type=="Table"){
h9.col=h9.colCount=-1;h9.row=h9.rowCount=-1;
}
}
this.Margin=function (left,top,right,bottom){
this.left;
this.top;
this.right;
this.bottom;
}
this.GetRender=function (h4){
var g0=h4;
if (g0.firstChild!=null&&g0.firstChild.tagName!=null&&g0.firstChild.tagName!="BR")
return g0.firstChild;
if (g0.firstChild!=null&&g0.firstChild.value!=null){
g0=g0.firstChild;
}
return g0;
}
this.GetPreferredRowHeight=function (e4,h1){
var j0=this.CreateTestBox(e4);
h1=this.GetDisplayIndex(e4,h1);
var i6=this.GetViewport(e4);
var j1=0;
var r3=i6.rows[h1].offsetHeight;
var f0=i6.rows[h1].cells.length;
for (var f1=0;f1<f0;f1++){
var j2=i6.rows[h1].cells[f1];
var j4=this.GetRender(j2);
if (j4!=null){
j0.style.fontFamily=j4.style.fontFamily;
j0.style.fontSize=j4.style.fontSize;
j0.style.fontWeight=j4.style.fontWeight;
j0.style.fontStyle=j4.style.fontStyle;
}
var n9=this.GetColFromCell(e4,j2);
j0.style.posWidth=this.GetColWidthFromCol(e4,n9);
if (j4!=null&&j4.tagName=="SELECT"){
var g0="";
for (var i1=0;i1<j4.childNodes.length;i1++){
var r4=j4.childNodes[i1];
if (r4.text!=null&&r4.text.length>g0.length)g0=r4.text;
}
j0.innerHTML=g0;
}
else if (j4!=null&&j4.tagName=="INPUT")
j0.innerHTML=j4.value;
else 
{
j0.innerHTML=j2.innerHTML;
}
r3=j0.offsetHeight;
if (r3>j1)j1=r3;
}
return Math.max(0,j1)+3;
}
this.SetRowHeight2=function (e4,h1,height){
if (height<1){
height=1;
}
h1=this.GetDisplayIndex(e4,h1);
var b5=null;
var g7=false;
if (h1<e4.frzRows){
g7=true;
if (this.GetFrozRowHeader(e4)!=null)b5=this.GetFrozRowHeader(e4).rows[h1];
}else {
h1-=e4.frzRows;
if (this.GetRowHeader(e4)!=null)b5=this.GetRowHeader(e4).rows[h1];
}
if (b5!=null)b5.style.height=""+height+"px";
if (g7){
var i6=this.GetViewport0(e4);
if (i6!=null){
if (b5!=null){
i6.rows[b5.rowIndex].style.height=""+(b5.offsetHeight-i6.rows[0].offsetTop)+"px";
}else {
i6.rows[h1].style.height=""+height+"px";
b5=i6.rows[h1];
}
}
i6=this.GetViewport1(e4);
if (i6!=null){
if (b5!=null){
i6.rows[b5.rowIndex].style.height=""+(b5.offsetHeight-i6.rows[0].offsetTop)+"px";
}else {
i6.rows[h1].style.height=""+height+"px";
b5=i6.rows[h1];
}
}
}else {
var i6=this.GetViewport(e4);
if (i6!=null){
if (b5!=null){
i6.rows[b5.rowIndex].style.height=b5.style.height;
}else {
i6.rows[h1].style.height=""+height+"px";
b5=i6.rows[h1];
}
}
i6=this.GetViewport2(e4);
if (i6!=null){
if (b5!=null){
i6.rows[b5.rowIndex].style.height=b5.style.height;
}else {
i6.rows[h1].style.height=""+height+"px";
b5=i6.rows[h1];
}
}
}
var r5=this.AddRowInfo(e4,b5.getAttribute("FpKey"));
if (r5!=null){
if (typeof(b5.style.posHeight)=="undefined")
b5.style.posHeight=height;
this.SetRowHeight(e4,r5,b5.style.posHeight);
}
var i8=this.GetParentSpread(e4);
if (i8!=null)i8.UpdateRowHeight(e4);
this.SizeSpread(e4);
}
this.GetRowHeightInternal=function (e4,h1){
var b5=null;
if (this.GetRowHeader(e4)!=null)
b5=this.GetRowHeader(e4).rows[h1];
else if (this.GetViewport(e4)!=null)
b5=this.GetViewport(e4).rows[h1];
if (b5!=null)
return b5.offsetHeight;
else 
return 0;
}
this.GetCell=function (ele,noHeader,event){
var g0=ele;
while (g0!=null){
if (noHeader){
if ((g0.tagName=="TD"||g0.tagName=="TH")&&(g0.parentNode.getAttribute("FpSpread")=="r")){
return g0;
}
}else {
if ((g0.tagName=="TD"||g0.tagName=="TH")&&(g0.parentNode.getAttribute("FpSpread")=="r"||g0.parentNode.getAttribute("FpSpread")=="ch"||g0.parentNode.getAttribute("FpSpread")=="rh")){
return g0;
}
}
g0=g0.parentNode;
}
return null;
}
this.InRowHeader=function (e4,h4){
return (this.IsChild(h4,this.GetFrozRowHeader(e4))||this.IsChild(h4,this.GetRowHeader(e4)));
}
this.InColHeader=function (e4,h4){
return (this.IsChild(h4,this.GetFrozColHeader(e4))||this.IsChild(h4,this.GetColHeader()));
}
this.InColFooter=function (e4,h4){
return (this.IsChild(h4,this.GetFrozColFooter(e4))||this.IsChild(h4,this.GetColFooter()));
}
this.IsHeaderCell=function (e4,h4){
return (h4!=null&&(h4.tagName=="TD"||h4.tagName=="TH")&&(h4.parentNode.getAttribute("FpSpread")=="ch"||h4.parentNode.getAttribute("FpSpread")=="rh"));
}
this.InFrozCols=function (e4,h4){
return (this.IsChild(h4,this.GetFrozColHeader(e4))||this.IsChild(h4,this.GetViewport0(e4))||this.IsChild(h4,this.GetViewport2(e4)));
}
this.InFrozRows=function (e4,h4){
(this.IsChild(h4,this.GetFrozRowHeader(e4))||this.IsChild(h4,this.GetViewport0(e4))||this.IsChild(h4,this.GetViewport1(e4)));
}
this.GetSizeColumn=function (e4,ele,event){
if (ele.tagName!="TD"||(this.GetColHeader(e4)==null))return null;
var n9=-1;
var g0=ele;
var r1=this.GetViewport(this.GetTopSpread(e4)).parentNode.scrollLeft+window.scrollX;
while (g0!=null&&g0.parentNode!=null&&g0.parentNode!=document.documentElement){
if (g0.parentNode.getAttribute("FpSpread")=="ch"){
var r6=this.GetOffsetLeft(e4,g0,document.body);
var r7=r6+g0.offsetWidth;
if (event.clientX+r1<r6+3){
n9=this.GetColFromCell(e4,g0)-1;
}
else if (event.clientX+r1>r7-4){
n9=this.GetColFromCell(e4,g0);
var r8=this.GetSpanCell(g0.parentNode.rowIndex,n9,e4.e1);
if (r8!=null){
n9=r8.col+r8.colCount-1;
}
}else {
n9=this.GetColFromCell(e4,g0);
var r8=this.GetSpanCell(g0.parentNode.rowIndex,n9,e4.e1);
if (r8!=null){
var j3=r6;
n9=-1;
for (var f1=r8.col;f1<r8.col+r8.colCount&&f1<this.GetColCount(e4);f1++){
if (this.IsChild(g0,this.GetColHeader(e4)))
j3+=parseInt(this.GetElementById(this.GetColHeader(e4),e4.id+"col"+f1).width);
else 
j3+=parseInt(this.GetElementById(this.GetFrozColHeader(e4),e4.id+"col"+f1).width);
if (event.clientX>j3-3&&event.clientX<j3+3){
n9=f1;
break ;
}
}
}else {
n9=-1;
}
}
if (isNaN(n9)||n9<0)return null;
var r9=0;
var s0=this.GetColCount(e4);
var s1=true;
var e7=null;
var h3=n9+1;
while (h3<s0){
var f3=this.GetColGroup(this.GetColHeader(e4));
if (h3>=e4.frzCols){
var f3=this.GetColGroup(this.GetColHeader(e4));
if (h3-e4.frzCols<f3.childNodes.length)
r9=parseInt(f3.childNodes[h3-e4.frzCols].width);
}else {
var f3=this.GetColGroup(this.GetFrozColHeader(e4));
if (h3<f3.childNodes.length)
r9=parseInt(f3.childNodes[h3].width);
}
if (r9>1){
s1=false;
break ;
}
h3++;
}
if (s1){
h3=n9+1;
while (h3<s0){
if (this.GetSizable(e4,h3)){
n9=h3;
break ;
}
h3++;
}
}
if (!this.GetSizable(e4,n9))return null;
if (this.IsChild(g0,this.GetColHeader(e4))){
if (event.offsetX<3&&g0.cellIndex==0&&this.GetFrozColHeader(e4)!=null){
return this.GetElementById(this.GetFrozColHeader(e4),e4.id+"col"+(e4.frzCols-1));
}else {
return this.GetElementById(this.GetColHeader(e4),e4.id+"col"+n9);
}
}else {
return this.GetElementById(this.GetFrozColHeader(e4),e4.id+"col"+n9);
}
}
g0=g0.parentNode;
}
return null;
}
this.GetColGroup=function (g0){
if (g0==null)return null;
var f3=g0.getElementsByTagName("COLGROUP");
if (f3!=null&&f3.length>0){
if (g0.colgroup!=null)return g0.colgroup;
var q1=new Object();
q1.childNodes=new Array();
for (var f1=0;f1<f3[0].childNodes.length;f1++){
if (f3[0].childNodes[f1]!=null&&f3[0].childNodes[f1].tagName=="COL"){
var f0=q1.childNodes.length;
q1.childNodes.length++;
q1.childNodes[f0]=f3[0].childNodes[f1];
}
}
g0.colgroup=q1;
return q1;
}else {
return null;
}
}
this.GetSizeRow=function (e4,ele,event){
var n6=this.GetRowCount(e4);
if (n6==0)return null;
if (e4.getAttribute("LayoutMode"))return null;
var h4=this.GetCell(ele);
if (h4==null){
if (ele.getAttribute("FpSpread")=="rowpadding"){
if (event.clientY<3){
var f0=ele.parentNode.rowIndex;
if (f0>1){
var i9=ele.parentNode.parentNode.rows[f0-1];
if (this.GetSizable(e4,i9))
return i9;
}
}
}
var c4=this.GetCorner(e4);
if (c4!=null&&this.IsChild(ele,c4)){
if (event.clientY>ele.offsetHeight-4){
var s2=null;
var f0=0;
s2=this.GetRowHeader(e4);
if (s2!=null){
while (f0<s2.rows.length&&s2.rows[f0].offsetHeight<2&&!this.GetSizable(e4,s2.rows[f0]))
f0++;
if (f0<s2.rows.length&&this.GetSizable(e4,s2.rows[f0])&&s2.rows[f0].offsetHeight<2)
return s2.rows[f0];
}
}else {
}
}
return null;
}
var e0=e4.e0;
var d9=e4.d9;
var s3=this.IsChild(h4,this.GetFrozRowHeader(e4));
var g0=h4;
var r0=this.GetViewport(this.GetTopSpread(e4)).parentNode.scrollTop+window.scrollY;
while (g0!=null&&g0!=document.documentElement){
if (g0.getAttribute("FpSpread")=="rh"){
var f0=-1;
var s4=this.GetOffsetTop(e4,g0,document.body);
var s5=s4+g0.offsetHeight;
if (event.clientY+r0<s4+3){
if (g0.rowIndex>1)
f0=g0.rowIndex-1;
else if (g0.rowIndex==0&&!s3&&this.GetFrozRowHeader(e4)!=null){
s3=true;
f0=g0.frzRows-1;
}
}
else if (event.clientY+r0>s5-4){
var r8=this.GetSpanCell(this.GetRowFromCell(e4,h4),this.GetColFromCell(e4,h4),e0);
if (r8!=null){
var k0=s4;
for (var f1=r8.row;f1<r8.row+r8.rowCount;f1++){
k0+=parseInt(this.GetRowHeader(e4).rows[f1].style.height);
if (event.clientY>k0-3&&event.clientY<k0+3){
f0=f1;
break ;
}
}
}else {
if (g0.rowIndex>=0)f0=g0.rowIndex;
}
}
else {
break ;
}
var k0=0;
var n6=this.GetRowHeader(e4).rows.length;
if (s3)n6=this.GetFrozRowHeader(e4).rows.length;
var s6=true;
var s2=null;
if (s3)
s2=this.GetFrozRowHeader(e4);
else 
s2=this.GetRowHeader(e4);
var h1=f0+1;
while (h1<n6){
if (s2.rows[h1].style.height!=null)k0=parseInt(s2.rows[h1].style.height);
else k0=parseInt(s2.rows[h1].offsetHeight);
if (k0>1){
s6=false;
break ;
}
h1++;
}
if (s6){
h1=f0+1;
while (h1<n6){
if (this.GetSizable(e4,this.GetRowHeader(e4).rows[h1])){
f0=h1;
break ;
}
h1++;
}
}
if (f0>=0&&this.GetSizable(e4,s2.rows[f0])){
return s2.rows[f0];
}
else if (event.clientY<3){
while (f0>0&&s2.rows[f0].offsetHeight==0&&!this.GetSizable(e4,s2.rows[f0]))
f0--;
if (f0>=0&&this.GetSizable(e4,s2.rows[f0]))
return s2.rows[f0];
else 
return null;
}
}
g0=g0.parentNode;
}
return null;
}
this.GetElementById=function (i8,id){
if (i8==null)return null;
var g0=i8.firstChild;
while (g0!=null){
if (g0.id==id||(typeof(g0.getAttribute)=="function"&&g0.getAttribute("name")==id))return g0;
var q1=this.GetElementById(g0,id)
if (q1!=null)return q1;
g0=g0.nextSibling;
}
return null;
}
this.GetSizable=function (e4,ele){
if (typeof(ele)=="number"){
var h4=null;
if (ele<e4.frzCols)
h4=this.GetElementById(this.GetFrozColHeader(e4),e4.id+"col"+ele);
else 
h4=this.GetElementById(this.GetColHeader(e4),e4.id+"col"+ele);
return (h4!=null&&(h4.getAttribute("Sizable")==null||h4.getAttribute("Sizable")=="True"));
}
return (ele!=null&&(ele.getAttribute("Sizable")==null||ele.getAttribute("Sizable")=="True"));
}
this.GetSpanWidth=function (e4,n9,s0){
var j3=0;
var e7=this.GetViewport(e4);
if (e7!=null){
var f3=this.GetColGroup(e7);
if (f3!=null){
for (var f1=n9;f1<n9+s0;f1++){
j3+=parseInt(f3.childNodes[f1].width);
}
}
}
return j3;
}
this.GetCellType=function (h4){
if (h4!=null&&h4.getAttribute("FpCellType")!=null)return h4.getAttribute("FpCellType");
if (h4!=null&&h4.getAttribute("FpRef")!=null){
var g0=document.getElementById(h4.getAttribute("FpRef"));
return g0.getAttribute("FpCellType");
}
if (h4!=null&&h4.getAttribute("FpCellType")!=null)return h4.getAttribute("FpCellType");
return "text";
}
this.GetCellType2=function (h4){
if (h4!=null&&h4.getAttribute("FpRef")!=null){
h4=document.getElementById(h4.getAttribute("FpRef"));
}
var j5=null;
if (h4!=null){
j5=h4.getAttribute("FpCellType");
if (j5=="readonly")j5=h4.getAttribute("CellType");
if (j5==null&&h4.getAttribute("CellType2")=="TagCloudCellType")
j5=h4.getAttribute("CellType2");
}
if (j5!=null)return j5;
return "text";
}
this.GetCellEditorID=function (e4,h4){
if (h4!=null&&h4.getAttribute("FpRef")!=null){
var g0=document.getElementById(h4.getAttribute("FpRef"));
return g0.getAttribute("FpEditorID");
}
if (h4.getAttribute("FpEditorID")!=null)
return h4.getAttribute("FpEditorID");
return e4.getAttribute("FpDefaultEditorID");
}
this.EditorMap=function (editorID,a8){
this.id=editorID;
this.a8=a8;
}
this.ValidatorMap=function (validatorID,validator){
this.id=validatorID;
this.validator=validator;
}
this.GetCellEditor=function (e4,editorID,noClone){
var a8=null;
for (var f1=0;f1<this.c0.length;f1++){
var s7=this.c0[f1];
if (s7.id==editorID){
a8=s7.a8;
break ;
}
}
if (a8==null){
a8=document.getElementById(editorID);
this.c0[this.c0.length]=new this.EditorMap(editorID,a8);
}
if (noClone)
return a8;
return a8.cloneNode(true);
}
this.GetCellValidatorID=function (e4,h4){
return null;
}
this.GetCellValidator=function (e4,validatorID){
return null;
}
this.GetTableRow=function (e4,h1){
var f7=this.GetData(e4).getElementsByTagName("root")[0];
var f6=f7.getElementsByTagName("data")[0];
var g0=f6.firstChild;
while (g0!=null){
if (g0.getAttribute("key")==""+h1)return g0;
g0=g0.nextSibling;
}
return null;
}
this.GetTableCell=function (i9,h3){
if (i9==null)return null;
var g0=i9.firstChild;
while (g0!=null){
if (g0.getAttribute("key")==""+h3)return g0;
g0=g0.nextSibling;
}
return null;
}
this.AddTableRow=function (e4,h1){
if (h1==null)return null;
var o1=this.GetTableRow(e4,h1);
if (o1!=null)return o1;
var f7=this.GetData(e4).getElementsByTagName("root")[0];
var f6=f7.getElementsByTagName("data")[0];
if (document.all!=null){
o1=this.GetData(e4).createNode("element","row","");
}else {
o1=document.createElement("row");
o1.style.display="none";
}
o1.setAttribute("key",h1);
f6.appendChild(o1);
return o1;
}
this.AddTableCell=function (i9,h3){
if (i9==null)return null;
var o1=this.GetTableCell(i9,h3);
if (o1!=null)return o1;
if (document.all!=null){
o1=this.GetData(e4).createNode("element","cell","");
}else {
o1=document.createElement("cell");
o1.style.display="none";
}
o1.setAttribute("key",h3);
i9.appendChild(o1);
return o1;
}
this.GetCellValue=function (e4,h4){
if (h4==null)return null;
var h1=this.GetRowKeyFromCell(e4,h4);
var h3=e4.getAttribute("LayoutMode")?this.GetColKeyFromCell2(e4,h4):this.GetColKeyFromCell(e4,h4);
var s8=this.AddTableCell(this.AddTableRow(e4,h1),h3);
return s8.innerHTML;
}
this.HTMLEncode=function (s){
var s9=new String(s);
var t0=new RegExp("&","g");
s9=s9.replace(t0,"&amp;");
t0=new RegExp("<","g");
s9=s9.replace(t0,"&lt;");
t0=new RegExp(">","g");
s9=s9.replace(t0,"&gt;");
t0=new RegExp("\"","g");
s9=s9.replace(t0,"&quot;");
return s9;
}
this.HTMLDecode=function (s){
var s9=new String(s);
var t0=new RegExp("&amp;","g");
s9=s9.replace(t0,"&");
t0=new RegExp("&lt;","g");
s9=s9.replace(t0,"<");
t0=new RegExp("&gt;","g");
s9=s9.replace(t0,">");
t0=new RegExp("&nbsp;","g");
s9=s9.replace(t0," ");
t0=new RegExp("&quot;","g");
s9=s9.replace(t0,'"');
return s9;
}
this.SetCellValue=function (e4,h4,val,noEvent,recalc){
if (h4==null)return ;
var t1=this.GetCellType(h4);
if (t1=="readonly")return ;
var h1=this.GetRowKeyFromCell(e4,h4);
var h3=e4.getAttribute("LayoutMode")?this.GetColKeyFromCell2(e4,h4):this.GetColKeyFromCell(e4,h4);
var s8=this.AddTableCell(this.AddTableRow(e4,h1),h3);
val=this.HTMLEncode(val);
val=this.HTMLEncode(val);
s8.innerHTML=val;
if (!noEvent){
var g3=this.CreateEvent("DataChanged");
g3.cell=h4;
g3.cellValue=val;
g3.row=h1;
g3.col=h3;
this.FireEvent(e4,g3);
}
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.GetSelectedRanges=function (e4){
var o0=this.GetSelection(e4);
var g5=new Array();
var o1=o0.firstChild;
while (o1!=null){
var h9=new this.Range();
this.GetRangeFromNode(e4,o1,h9);
var g0=g5.length;
g5.length=g0+1;
g5[g0]=h9;
o1=o1.nextSibling;
}
return g5;
}
this.GetSelectedRange=function (e4){
var h9=new this.Range();
var o0=this.GetSelection(e4);
var o1=o0.lastChild;
if (o1!=null){
this.GetRangeFromNode(e4,o1,h9);
}
return h9;
}
this.GetRangeFromNode=function (e4,o1,h9){
if (o1==null||e4==null||h9==null)return ;
var h1;
var h3;
if (e4.getAttribute("LayoutMode")){
h1=parseInt(o1.getAttribute("rowIndex"));
h3=parseInt(o1.getAttribute("colIndex"));
}
else {
h1=this.GetRowByKey(e4,o1.getAttribute("row"));
h3=this.GetColByKey(e4,o1.getAttribute("col"));
}
var n6=parseInt(o1.getAttribute("rowcount"));
var h0=parseInt(o1.getAttribute("colcount"));
var i6=this.GetViewport(e4);
if (i6!=null){
var t2=this.GetDisplayIndex(e4,h1);
for (var f1=t2;f1<t2+n6;f1++){
if (this.IsChildSpreadRow(e4,i6,f1))n6--;
}
}
var t3;
if (e4.getAttribute("LayoutMode")){
var n9=parseInt(o1.getAttribute("col"));
if (n9!=-1&&parseInt(o1.getAttribute("row"))==-1&&n6==-1){
h1=parseInt(o1.getAttribute("row"));
t3=parseInt(o1.getAttribute("rowIndex"));
}
}
var t4=null;
if (h1<0&&h3<0&&n6!=0&&h0!=0)
t4="Table";
else if (h1<0&&h3>=0&&h0>0)
t4="Column";
else if (h3<0&&h1>=0&&n6>0)
t4="Row";
else 
t4="Cell";
this.SetRange(h9,t4,h1,h3,n6,h0,t3);
}
this.GetSelection=function (e4){
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var t5=o4.getElementsByTagName("selection")[0];
return t5;
}
this.GetRowKeyFromRow=function (e4,h1){
if (h1<0)return null;
var e7=null;
if (h1<e4.frzRows){
e7=this.GetViewport0(e4);
if (e7==null)e7=this.GetViewport1(e4);
}else {
e7=this.GetViewport2(e4);
if (e7==null)e7=this.GetViewport(e4);
}
if (h1>=e4.frzRows){
h1-=e4.frzRows;
}
return e7.rows[h1].getAttribute("FpKey");
}
this.GetColKeyFromCol=function (e4,h3){
if (h3<0)return null;
var e7=null;
if (h3>=e4.frzCols){
e7=this.GetViewport1(e4);
if (e7==null)e7=this.GetViewport(e4);
if (e7==null)e7=this.GetColHeader(e4);
}else {
e7=this.GetViewport0(e4);
if (e7==null)e7=this.GetViewport2(e4);
if (e7==null)e7=this.GetFrozColHeader(e4);
}
if (h3>=e4.frzCols)
h3=h3-e4.frzCols;
var f3=this.GetColGroup(e7);
if (f3!=null&&h3>=0&&h3<f3.childNodes.length){
return f3.childNodes[h3].getAttribute("FpCol");
}
return null;
}
this.GetRowKeyFromCell=function (e4,h4){
var h1=h4.parentNode.getAttribute("FpKey");
return h1;
}
this.GetColKeyFromCell=function (e4,h4){
var n9=this.GetColFromCell(e4,h4);
if (n9>=e4.frzCols){
e7=this.GetViewport(e4);
if (e7==null||!this.IsChild(h4,e7))e7=this.GetViewport1(e4);
var f3=this.GetColGroup(e7);
if (f3!=null&&n9-e4.frzCols>=0&&n9-e4.frzCols<f3.childNodes.length){
return f3.childNodes[n9-e4.frzCols].getAttribute("FpCol");
}
}else {
e7=this.GetViewport0(e4);
if (e7==null||!this.IsChild(h4,e7))e7=this.GetViewport2(e4);
var f3=this.GetColGroup(e7);
if (f3!=null&&n9>=0&&n9<f3.childNodes.length){
return f3.childNodes[n9].getAttribute("FpCol");
}
}
}
this.GetRowTemplateRowFromGroupCell=function (e4,h4,isColHeader){
var t6=this.GetColCount(e4);
var c6=this.GetColHeader(e4);
if ((!e4.allowGroup||isColHeader)&&c6!=null){
for (var f1=0;f1<c6.rows.length;f1++){
for (var i1=0;i1<t6;i1++){
var t7=c6.rows[f1].cells[i1];
var t8=isNaN(h4)?parseInt(h4.getAttribute("col")):h4;
if (t7!=null&&h4!=null&&parseInt(t7.getAttribute("col"))==t8)
return f1;
}
}
}
var n5=this.GetViewport1(e4);
if (n5!=null){
for (var i9=0;i9<n5.rows.length;i9++){
for (var n9=0;n9<n5.rows[i9].cells.length;n9++){
var t9=isNaN(h4)?parseInt(h4.getAttribute("col")):h4;
if (parseInt(n5.rows[i9].cells[n9].getAttribute("col"))==t9&&n5.rows[i9].cells[n9].getAttribute("group")==null)
return parseInt(n5.rows[i9].getAttribute("FpKey"));
}
}
}
var n7=this.GetViewport(e4);
if (n7!=null){
for (var i9=0;i9<n7.rows.length;i9++){
for (var n9=0;n9<n7.rows[i9].cells.length;n9++){
var t9=isNaN(h4)?parseInt(h4.getAttribute("col")):h4;
if (parseInt(n7.rows[i9].cells[n9].getAttribute("col"))==t9&&n7.rows[i9].cells[n9].getAttribute("group")==null)
return parseInt(n7.rows[i9].getAttribute("FpKey"));
}
}
}
return -1;
}
this.GetColTemplateRowFromGroupCell=function (e4,colIndex){
var t6=this.GetColCount(e4);
var c6=this.GetColHeader(e4);
var u0=this.GetRowTemplateRowFromGroupCell(e4,colIndex);
var h4=null
if (c6==null)return -1;
for (var f1=0;f1<c6.rows.length;f1++){
for (var i1=0;i1<t6;i1++){
var t7=c6.rows[f1].cells[i1];
if (t7!=null&&parseInt(t7.getAttribute("col"))==colIndex){
h4=t7;
break ;
}
}
}
return this.GetColFromCell(e4,h4);
}
this.GetColKeyFromCell2=function (e4,h4){
if (!h4)return -1;
if (h4.getAttribute("col"))
return h4.getAttribute("col")=="-1"?0:parseInt(h4.getAttribute("col"));
else 
return this.GetColKeyFromCell(e4,h4);
}
this.GetColKeyFromCol2=function (e4,i9,n9){
var h4=this.GetCellFromRowCol(e4,i9,n9);
if (h4)
return this.GetColKeyFromCell2(e4,h4);
return n9;
}
this.GetCellByRowCol2=function (e4,h1,h3){
if (h1==null||h3==null||h1.length<=0||h1=="-1"||h3.length<=0||h3=="-1")
return null;
var f4=this.GetViewport1(e4);
if (f4!=null){
for (var i9=0;i9<f4.rows.length;i9++){
if (f4.rows[i9].getAttribute("FpKey")==h1){
for (var n9=0;n9<f4.rows[i9].cells.length;n9++){
if (f4.rows[i9].cells[n9].getAttribute("col")==h3)
return f4.rows[i9].cells[n9];
}
}
}
}
var n7=this.GetViewport(e4);
if (n7!=null){
for (var i9=0;i9<n7.rows.length;i9++){
if (n7.rows[i9].getAttribute("FpKey")==h1){
for (var n9=0;n9<n7.rows[i9].cells.length;n9++){
if (n7.rows[i9].cells[n9].getAttribute("col")==h3)
return n7.rows[i9].cells[n9];
}
}
}
}
return null;
}
this.GetRowTemplateRowFromCell=function (e4,h4){
if (!h4)return -1;
try {
var h1
if (h4.getAttribute("group")!=null)
h1=this.GetRowTemplateRowFromGroupCell(e4,h4);
else 
h1=parseInt(h4.parentNode.getAttribute("row"));
return h1;
}
catch (g3){
return -1;
}
}
this.PaintMultipleRowSelection=function (e4,h1,h3,n6,h0,select){
var u1=this.GetRowCountInternal(e4);
var t6=this.GetColCount(e4);
var u2=true;
for (var f1=h1;f1<u1;f1++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),f1))continue ;
var h4=null;
for (var i1=h3;i1<h3+h0&&i1<t6;i1++){
if (this.IsCovered(e4,f1,i1,e4.d9))continue ;
h4=this.GetCellFromRowCol(e4,f1,i1,h4);
if (h4!=null&&parseInt(h4.parentNode.getAttribute("row"))==h1){
this.PaintViewportSelection(e4,f1,i1,n6,h0,select);
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal"&&u2)this.PaintHeaderSelection(e4,f1,i1,n6,h0,select,true);
if (this.GetRowHeader(e4)!=null)this.PaintHeaderSelection(e4,f1,i1,n6,h0,select,false);
u2=false;
}
}
}
this.PaintAnchorCell(e4);
}
this.GetFirstRowFromKey=function (e4,rowKey){
var f4=this.GetViewport1(e4)
if (f4!=null){
for (var i9=0;i9<f4.rows.length;i9++){
if (f4.rows[i9].getAttribute("FpKey")==rowKey){
return i9;
}
}
}
var n7=this.GetViewport(e4)
if (n7!=null){
for (var i9=0;i9<n7.rows.length;i9++){
if (n7.rows[i9].getAttribute("FpKey")==rowKey){
return (e4.frzRows!=null)?e4.frzRows+i9:i9;
}
}
}
return null;
}
this.GetFirstMultiRowFromViewport=function (e4,i9,isColHeader){
var e7=null;
var u3=null;
if (i9<e4.frzRows)
e7=this.GetViewport1(e4);
else 
e7=this.GetViewport(e4);
if (!isColHeader)
u3=this.GetRowKeyFromRow(e4,i9);
var u4=parseInt(e4.getAttribute("layoutrowcount"));
var u5;
for (var f1=0;f1<e7.rows.length;f1++){
u5=0;
if (u3!=null){
if (e7.rows[f1].getAttribute("FpKey")==u3)
return ((e4.frzRows!=null&&i9<e4.frzRows)?f1:f1+e4.frzRows);
}
else {
for (var i1=f1+1;i1<e7.rows.length;i1++){
if (e7.rows[f1]!=null&&e7.rows[i1]!=null&&e7.rows[f1].getAttribute("FpKey")==e7.rows[i1].getAttribute("FpKey"))
u5++;
if (u5==(u4-1))
return f1;
}
}
}
}
this.GetRowFromViewPort=function (e4,h1,h3){
if (h1<0||h3<0)return null;
var e7=null;
if (h1<e4.frzRows)
e7=this.GetViewport1(e4);
else 
e7=this.GetViewport(e4);
if (h1>=0&&h1<e7.rows.length){
for (var f1=0;f1<e7.rows.length;f1++){
if (e7.rows[f1].getAttribute("row")!=null&&parseInt(e7.rows[f1].getAttribute("row"))==h1)
return f1;
}
}
return 0;
}
this.GetDisplayIndex2=function (e4,u0){
if (!e4.allowGroup)
return (u0!=null)?this.GetDisplayIndex(e4,u0):0;
else {
var f4=this.GetViewport1(e4);
if (f4!=null){
for (var i9=u0;i9<f4.rows.length;i9++){
if (f4.rows(i9).getAttribute("row")==u0){
return i9;
}
}
}
var n7=this.GetViewport(e4);
if (n7!=null){
for (var i9=u0;i9<n7.rows.length;i9++){
if (IsChildSpreadRow(n7,i9))continue ;
if (n7.rows(i9).getAttribute("row")==u0){
return i9;
}
}
}
}
return -1;
}
this.SetSelection=function (e4,i9,n9,rowcount,colcount,addSelection,rowIndex2,colIndex2){
if (!e4.initialized)return ;
var u0=i9;
var u6=(colIndex2==null)?n9:colIndex2;
if (i9!=null&&parseInt(i9)>=0){
i9=this.GetRowKeyFromRow(e4,i9);
if (i9!="newRow")
i9=parseInt(i9);
}
if (n9!=null&&parseInt(n9)>=0){
if (e4.getAttribute("LayoutMode"))
n9=parseInt(this.GetColKeyFromCol2(e4,u0,n9));
else 
n9=parseInt(this.GetColKeyFromCol(e4,n9));
}
if (e4.getAttribute("LayoutMode")&&rowIndex2!=null)
u0=rowIndex2;
var o1=this.GetSelection(e4);
if (o1==null)return ;
if (addSelection==null)
addSelection=(e4.getAttribute("multiRange")=="true"&&!this.a6);
var u7=o1.lastChild;
if (u7==null||addSelection){
if (document.all!=null){
u7=this.GetData(e4).createNode("element","range","");
}else {
u7=document.createElement('range');
u7.style.display="none";
}
o1.appendChild(u7);
}
u7.setAttribute("row",i9);
u7.setAttribute("col",n9);
u7.setAttribute("rowcount",rowcount);
u7.setAttribute("colcount",colcount);
u7.setAttribute("rowIndex",u0);
u7.setAttribute("colIndex",u6);
e4.e2=true;
this.PaintFocusRect(e4);
var f9=this.GetCmdBtn(e4,"Update");
this.UpdateCmdBtnState(f9,false);
var g3=this.CreateEvent("SelectionChanged");
this.FireEvent(e4,g3);
}
this.CreateSelectionNode=function (e4,i9,n9,rowcount,colcount,u0,u6){
var u7=document.createElement('range');
u7.style.display="none";
u7.setAttribute("row",i9);
u7.setAttribute("col",n9);
u7.setAttribute("rowcount",rowcount);
u7.setAttribute("colcount",colcount);
u7.setAttribute("rowIndex",u0);
u7.setAttribute("colIndex",u6);
return u7;
}
this.AddRowToSelection=function (e4,o1,i9){
var o2=this.GetOperationMode(e4);
if (e4.getAttribute("LayoutMode")&&(o2=="ExtendedSelect"||o2=="MultiSelect"))return ;
var u0=i9;
if (typeof(i9)!="undefined"&&parseInt(i9)>=0){
i9=this.GetRowKeyFromRow(e4,i9);
if (i9!="newRow")
i9=parseInt(i9);
}
if (!this.IsRowSelected(e4,i9)&&!isNaN(i9))
{
var u7=this.CreateSelectionNode(e4,i9,-1,1,-1,u0,-1);
o1.appendChild(u7);
}
}
this.RemoveSelection=function (e4,i9,n9,rowcount,colcount){
var o1=this.GetSelection(e4);
if (o1==null)return ;
var u7=o1.firstChild;
while (u7!=null){
var h1;
var n6;
var o2=this.GetOperationMode(e4);
if (e4.getAttribute("LayoutMode")&&(o2=="ExtendedSelect"||o2=="MultiSelect")){
var o3=parseInt(u7.getAttribute("row"));
h1=this.GetFirstRowFromKey(e4,o3);
}
else 
h1=parseInt(u7.getAttribute("rowIndex"));
if (e4.getAttribute("LayoutMode")&&(o2=="ExtendedSelect"||o2=="MultiSelect"))
n6=parseInt(e4.getAttribute("layoutrowcount"));
else 
n6=parseInt(u7.getAttribute("rowcount"));
if (h1<=i9&&i9<h1+n6){
o1.removeChild(u7);
for (var f1=h1;f1<h1+n6;f1++){
if (f1!=i9){
this.AddRowToSelection(e4,o1,f1);
}
}
break ;
}
u7=u7.nextSibling;
}
e4.e2=true;
var f9=this.GetCmdBtn(e4,"Update");
this.UpdateCmdBtnState(f9,false);
var g3=this.CreateEvent("SelectionChanged");
this.FireEvent(e4,g3);
}
this.GetColInfo=function (e4,h3){
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var n9=o4.getElementsByTagName("colinfo")[0];
var g0=n9.firstChild;
while (g0!=null){
if (g0.getAttribute("key")==""+h3)return g0;
g0=g0.nextSibling;
}
return null;
}
this.GetColWidthFromCol=function (e4,h3){
var f3=this.GetColGroup(this.GetViewport(e4));
return parseInt(f3.childNodes[h3].width);
}
this.GetColWidth=function (colInfo){
if (colInfo==null)return null;
var o1=colInfo.getElementsByTagName("width")[0];
if (o1!=null)return o1.innerHTML;
return 0;
}
this.AddColInfo=function (e4,h3){
var o1=this.GetColInfo(e4,h3);
if (o1!=null)return o1;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var n9=o4.getElementsByTagName("colinfo")[0];
if (document.all!=null){
o1=this.GetData(e4).createNode("element","col","");
}else {
o1=document.createElement('col');
o1.style.display="none";
}
o1.setAttribute("key",h3);
n9.appendChild(o1);
return o1;
}
this.SetColWidth=function (e4,n9,width,oldWidth){
if (n9==null)return ;
n9=parseInt(n9);
var j9=this.IsXHTML(e4);
var u8=null;
if (n9<e4.frzCols){
if (this.GetViewport0(e4)!=null){
var f3=this.GetColGroup(this.GetViewport0(e4));
if (f3==null||f3.childNodes.length==0){
f3=this.GetColGroup(this.GetFrozColHeader(e4));
}
u8=this.AddColInfo(e4,f3.childNodes[n9].getAttribute("FpCol"));
if (width==0)width=1;
if (f3!=null){
if (oldWidth==null)oldWidth=f3.childNodes[n9].width;
f3.childNodes[n9].width=width;
}
this.SetWidthFix(this.GetViewport0(e4),n9,width);
}
if (this.GetFrozColFooter(e4)!=null){
var f3=this.GetColGroup(this.GetFrozColFooter(e4));
if (f3==null||f3.childNodes.length==0){
f3=this.GetColGroup(this.GetFrozColHeader(e4));
}
if (width==0)width=1;
if (f3!=null){
if (oldWidth==null)oldWidth=f3.childNodes[n9].width;
f3.childNodes[n9].width=width;
}
this.SetWidthFix(this.GetFrozColFooter(e4),n9,width);
}
if (this.GetViewport2(e4)!=null){
var f3=this.GetColGroup(this.GetViewport2(e4));
if (f3==null||f3.childNodes.length==0){
f3=this.GetColGroup(this.GetFrozColHeader(e4));
}
u8=this.AddColInfo(e4,f3.childNodes[n9].getAttribute("FpCol"));
if (width==0)width=1;
if (f3!=null){
if (oldWidth==null)oldWidth=f3.childNodes[n9].width;
f3.childNodes[n9].width=width;
}
this.SetWidthFix(this.GetViewport2(e4),n9,width);
}
if (this.GetFrozColHeader(e4)!=null){
var u9=parseInt(this.GetFrozColHeader(e4).parentNode.parentNode.style.width);
this.GetFrozColHeader(e4).parentNode.parentNode.style.width=(u9+width-oldWidth)+"px";
if (this.GetViewport(e4)!=null){
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (j9){
if (n9==this.colCount-1)width-=1;
}
}
}
if (width<=0)width=1;
document.getElementById(e4.id+"col"+n9).width=width;
this.SetWidthFix(this.GetFrozColHeader(e4),n9,width);
if (this.GetViewport(e4)!=null){
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (n9==this.GetColCount(e4)-1)width+=1;
}
}
}
}else {
if (this.GetViewport1(e4)!=null){
var f3=this.GetColGroup(this.GetViewport1(e4));
if (f3==null||f3.childNodes.length==0){
f3=this.GetColGroup(this.GetColHeader(e4));
}
u8=this.AddColInfo(e4,f3.childNodes[n9-e4.frzCols].getAttribute("FpCol"));
if (this.GetViewport1(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport1(e4).rules!="rows"){
if (n9==0)width-=1;
}
if (width==0)width=1;
if (f3!=null)
f3.childNodes[n9-e4.frzCols].width=width;
this.SetWidthFix(this.GetViewport1(e4),n9-e4.frzCols,width);
var c6=this.GetColHeader(e4);
var k2=this.GetColGroup(this.GetViewport(e4));
var k3=this.GetColGroup(c6);
if (k2!=null&&k2.childNodes.length>0&&k3!=null&&k3.childNodes.length>0){
f3=this.GetColGroup(this.GetColHeader(e4));
if (f3!=null){
if (n9==e4.frzCols&&e4.frzCols>0)
width=width+k2.childNodes[0].offsetLeft;
f3.childNodes[n9-e4.frzCols].width=width;
}
}
this.SetWidthFix(this.GetColHeader(e4),n9-e4.frzCols,width);
}
if (this.GetViewport(e4)!=null){
var f3=this.GetColGroup(this.GetViewport(e4));
if (f3==null||f3.childNodes.length==0){
f3=this.GetColGroup(this.GetColHeader(e4));
}
u8=this.AddColInfo(e4,f3.childNodes[n9-e4.frzCols].getAttribute("FpCol"));
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (n9==0)width-=1;
}
if (width==0)width=1;
if (f3!=null)
f3.childNodes[n9-e4.frzCols].width=width;
this.SetWidthFix(this.GetViewport(e4),n9-e4.frzCols,width);
}
if (this.GetColHeader(e4)!=null){
if (this.GetViewport(e4)!=null){
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (n9==e4.frzCols&&e4.frzCols>0)width-=1;
if (n9==this.colCount-1)width-=1;
}
}
if (width<=0)width=1;
document.getElementById(e4.id+"col"+n9).width=width;
this.SetWidthFix(this.GetColHeader(e4),n9-e4.frzCols,width);
if (this.GetViewport(e4)!=null){
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (n9==this.GetColCount(e4)-1)width+=1;
}
}
}
if (this.GetColFooter(e4)!=null){
var f3=this.GetColGroup(this.GetColFooter(e4));
if (f3==null||f3.childNodes.length==0){
f3=this.GetColGroup(this.GetColHeader(e4));
}
u8=this.AddColInfo(e4,f3.childNodes[n9-e4.frzCols].getAttribute("FpCol"));
if (this.GetColFooter(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetColFooter(e4).rules!="rows"){
if (n9==0)width-=1;
}
if (width==0)width=1;
if (f3!=null)
f3.childNodes[n9-e4.frzCols].width=width;
this.SetWidthFix(this.GetColFooter(e4),n9-e4.frzCols,width);
}
}
var e9=this.GetTopSpread(e4);
this.SizeAll(e9);
this.Refresh(e9);
if (n9<e4.frzCols&&this.GetFrozColHeader(e4)!=null){
var u9=parseInt(this.GetFrozColHeader(e4).parentNode.parentNode.style.width);
this.GetFrozColHeader(e4).parentNode.parentNode.style.width=(u9+width-oldWidth)+"px";
var v0=this.GetColGroup(this.GetTopTable(e4));
if (v0!=null){
var v1=this.GetFrozColHeader(e4).parentNode.parentNode.cellIndex;
var u9=parseInt(v0.childNodes[1].width);
v0.childNodes[v1].width=(u9+width-oldWidth)+"px";
}
}
if (u8!=null){
var o1=u8.getElementsByTagName("width");
if (o1!=null&&o1.length>0)
o1[0].innerHTML=width;
else {
if (document.all!=null){
o1=this.GetData(e4).createNode("element","width","");
}else {
o1=document.createElement('width');
o1.style.display="none";
}
u8.appendChild(o1);
o1.innerHTML=width;
}
}
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null)this.UpdateCmdBtnState(f9,false);
e4.e2=true;
}
this.SetWidthFix=function (e7,n9,width){
if (e7==null||e7.rows.length==0)return ;
var f1=0;
var v2=0;
var j2=e7.rows[0].cells[0];
var v3=j2.colSpan;
if (v3==null)v3=1;
while (n9>=v2+v3){
f1++;
v2=v2+v3;
j2=e7.rows[0].cells[f1];
v3=j2.colSpan;
if (v3==null)v3=1;
}
j2.width=width;
}
this.GetRowInfo=function (e4,h1){
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var i9=o4.getElementsByTagName("rowinfo")[0];
var g0=i9.firstChild;
while (g0!=null){
if (g0.getAttribute("key")==""+h1)return g0;
g0=g0.nextSibling;
}
return null;
}
this.GetRowHeight=function (r5){
if (r5==null)return null;
var v4=r5.getElementsByTagName("height");
if (v4!=null&&v4.length>0)return v4[0].innerHTML;
return 0;
}
this.AddRowInfo=function (e4,h1){
var o1=this.GetRowInfo(e4,h1);
if (o1!=null)return o1;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var i9=o4.getElementsByTagName("rowinfo")[0];
if (document.all!=null){
o1=this.GetData(e4).createNode("element","row","");
}else {
o1=document.createElement('row');
o1.style.display="none";
}
o1.setAttribute("key",h1);
i9.appendChild(o1);
return o1;
}
this.GetTopSpread=function (g3)
{
if (g3==null)return null;
var g5=this.GetSpread(g3);
if (g5==null)return null;
var g0=g5.parentNode;
while (g0!=null&&g0.tagName!="BODY")
{
if (g0.getAttribute("FpSpread")=="Spread"){
if (g0.getAttribute("hierView")=="true")
g5=g0;
else 
break ;
}
g0=g0.parentNode;
}
return g5;
}
this.GetParentSpread=function (e4)
{
var v5=e4.getAttribute("parentSpread");
if (v5!=null){
if (v5.length<=0)
return null;
else 
return document.getElementById(v5);
}
else {
try {
var g0=e4.parentNode;
while (g0!=null&&g0!=document&&g0.getAttribute("FpSpread")!="Spread")g0=g0.parentNode;
if (g0!=null&&g0!=document&&g0.getAttribute("hierView")=="true"){
e4.setAttribute("parentSpread",g0.id);
return g0;
}
else {
e4.setAttribute("parentSpread","");
return null;
}
}catch (g3){
e4.setAttribute("parentSpread","");
return null;
}
}
}
this.SetRowHeight=function (e4,r5,height){
if (r5==null)return ;
var o1=r5.getElementsByTagName("height");
if (o1!=null&&o1.length>0)
o1[0].innerHTML=height;
else {
if (document.all!=null){
o1=this.GetData(e4).createNode("element","height","");
}else {
o1=document.createElement('height');
o1.style.display="none";
}
r5.appendChild(o1);
o1.innerHTML=height;
}
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null)this.UpdateCmdBtnState(f9,false);
e4.e2=true;
}
this.SetActiveRow=function (e4,i9){
if (this.GetRowCount(e4)<1)return ;
if (i9==null)i9=-1;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var o5=o4.firstChild;
while (o5!=null&&o5.tagName!="activerow"&&o5.tagName!="ACTIVEROW"){
o5=o5.nextSibling;
}
if (o5!=null)
o5.innerHTML=""+i9;
if (i9!=null&&e4.getAttribute("IsNewRow")!="true"&&e4.getAttribute("AllowInsert")=="true"){
var f9=this.GetCmdBtn(e4,"Insert");
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Add");
this.UpdateCmdBtnState(f9,false);
}else {
var f9=this.GetCmdBtn(e4,"Insert");
this.UpdateCmdBtnState(f9,true);
f9=this.GetCmdBtn(e4,"Add");
this.UpdateCmdBtnState(f9,true);
}
if (i9!=null&&e4.getAttribute("IsNewRow")!="true"&&(e4.getAttribute("AllowDelete")==null||e4.getAttribute("AllowDelete")=="true")){
var f9=this.GetCmdBtn(e4,"Delete");
this.UpdateCmdBtnState(f9,(i9==-1));
}else {
var f9=this.GetCmdBtn(e4,"Delete");
this.UpdateCmdBtnState(f9,true);
}
e4.e2=true;
}
this.SetActiveCol=function (e4,n9){
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var o4=f7.getElementsByTagName("state")[0];
var o6=o4.firstChild;
while (o6!=null&&o6.tagName!="activecolumn"&&o6.tagName!="ACTIVECOLUMN"){
o6=o6.nextSibling;
}
if (o6!=null)
o6.innerHTML=""+parseInt(n9);
e4.e2=true;
}
this.GetEditor=function (h4){
if (h4==null)return null;
var t1=this.GetCellType(h4);
if (t1=="readonly")return null;
var i4=h4.getElementsByTagName("DIV");
if (t1=="MultiColumnComboBoxCellType"){
if (i4!=null&&i4.length>0){
var g0=i4[0];
g0.type="div";
return g0;
}
}
i4=h4.getElementsByTagName("INPUT");
if (i4!=null&&i4.length>0){
var g0=i4[0];
while (g0!=null&&g0.getAttribute&&g0.getAttribute("FpEditor")==null)
g0=g0.parentNode;
if (!g0.getAttribute)g0=null;
return g0;
}
i4=h4.getElementsByTagName("SELECT");
if (i4!=null&&i4.length>0){
var g0=i4[0];
return g0;
}
return null;
}
this.GetPageActiveSpread=function (){
var v6=document.documentElement.getAttribute("FpActiveSpread");
var g0=null;
if (v6!=null)g0=document.getElementById(v6);
return g0;
}
this.GetPageActiveSheetView=function (){
var v6=document.documentElement.getAttribute("FpActiveSheetView");
var g0=null;
if (v6!=null)g0=document.getElementById(v6);
return g0;
}
this.SetPageActiveSpread=function (e4){
if (e4==null)
document.documentElement.setAttribute("FpActiveSpread",null);
else {
document.documentElement.setAttribute("FpActiveSpread",e4.id);
document.documentElement.setAttribute("FpActiveSheetView",e4.id);
}
}
this.DoResize=function (event){
if (the_fpSpread.spreads==null)return ;
var f0=the_fpSpread.spreads.length;
for (var f1=0;f1<f0;f1++){
if (the_fpSpread.spreads[f1]!=null)the_fpSpread.SizeSpread(the_fpSpread.spreads[f1]);
}
}
this.DblClick=function (event){
var h4=this.GetCell(this.GetTarget(event),true,event);
var e4=this.GetSpread(h4);
if (h4!=null&&!this.IsHeaderCell(h4)&&this.GetOperationMode(e4)=="RowMode"&&this.GetEnableRowEditTemplate(e4)=="true"&&!e4.getAttribute("LayoutMode")){
var v7=h4.getElementsByTagName("DIV");
if (v7!=null&&v7.length>0&&v7[0].id==e4.id+"_RowEditTemplateContainer")return ;
this.Edit(e4,this.GetRowKeyFromCell(e4,h4));
var f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null)
this.UpdateCmdBtnState(f9,false);
return ;
}
if (h4!=null&&!this.IsHeaderCell(h4)&&h4==e4.d1)this.StartEdit(e4,h4);
}
this.GetEvent=function (g3){
if (g3!=null)return g3;
return window.event;
}
this.GetTarget=function (g3){
g3=this.GetEvent(g3);
if (g3.target==document){
if (g3.currentTarget!=null)return g3.currentTarget;
}
if (g3.target!=null)return g3.target;
return g3.srcElement;
}
this.StartEdit=function (e4,editCell){
var v8=this.GetOperationMode(e4);
if (v8=="SingleSelect"||v8=="ReadOnly"||this.a7)return ;
if (v8=="RowMode"&&this.GetEnableRowEditTemplate(e4)=="true"&&!e4.getAttribute("LayoutMode"))return ;
var h4=editCell;
if (h4==null)h4=e4.d1;
if (h4==null)return ;
this.b1=-1;
var i4=this.GetEditor(h4);
if (i4!=null){
this.a7=true;
this.a8=i4;
this.b1=1;
}
var j9=this.IsXHTML(e4);
if (h4!=null){
var h1=this.GetRowFromCell(e4,h4);
var h3=this.GetColFromCell(e4,h4);
var g3=this.CreateEvent("EditStart");
g3.cell=h4;
g3.row=this.GetSheetIndex(e4,h1);
g3.col=h3;
g3.cancel=false;
this.FireEvent(e4,g3);
if (g3.cancel)return ;
var t1=this.GetCellType(h4);
if (t1=="readonly")return ;
if (e4.d1!=h4){
e4.d1=h4;
this.SetActiveRow(e4,this.GetRowKeyFromCell(e4,h4));
this.SetActiveCol(e4,e4.getAttribute("LayoutMode")?this.GetColKeyFromCell2(h4):this.GetColKeyFromCell(e4,h4));
}
if (i4==null){
var j4=this.GetRender(h4);
var v9=this.GetValueFromRender(e4,j4);
if (v9==" ")v9="";
this.a9=v9;
this.b0=this.GetFormulaFromCell(h4);
if (this.b0!=null)v9=this.b0;
try {
if (j4!=h4){
j4.style.display="none";
}
else {
j4.innerHTML="";
}
}catch (g3){
return ;
}
var w0=this.GetCellEditorID(e4,h4);
if (w0!=null&&w0.length>0){
this.a8=this.GetCellEditor(e4,w0,true);
if (!this.a8.getAttribute("MccbId")&&!this.a8.getAttribute("Extenders"))
this.a8.style.display="inline";
else 
this.a8.style.display="block";
}else {
this.a8=document.createElement("INPUT");
this.a8.type="text";
}
this.a8.style.fontFamily=j4.style.fontFamily;
this.a8.style.fontSize=j4.style.fontSize;
this.a8.style.fontWeight=j4.style.fontWeight;
this.a8.style.fontStyle=j4.style.fontStyle;
this.a8.style.textDecoration=j4.style.textDecoration;
this.a8.style.position="";
if (j9){
var k6=h4.clientWidth-2;
var w1=parseInt(h4.style.paddingLeft);
if (!isNaN(w1))
k6-=w1;
w1=parseInt(h4.style.paddingRight);
if (!isNaN(w1))
k6-=w1;
this.a8.style.width=""+k6+"px";
}
else 
this.a8.style.width=h4.clientWidth-2;
this.SaveMargin(h4);
if (this.a8.tagName=="TEXTAREA")
this.a8.style.height=""+(h4.offsetHeight-4)+"px";
if ((this.a8.tagName=="INPUT"&&this.a8.type=="text")||this.a8.tagName=="TEXTAREA"){
if (this.a8.style.backgroundColor==""||this.a8.backColorSet!=null){
var w2="";
if (document.defaultView!=null&&document.defaultView.getComputedStyle!=null)w2=document.defaultView.getComputedStyle(h4,'').getPropertyValue("background-color");
if (w2!="")
this.a8.style.backgroundColor=w2;
else 
this.a8.style.backgroundColor=h4.bgColor;
this.a8.backColorSet=true;
}
if (this.a8.style.color==""||this.a8.colorSet!=null){
var w3="";
if (document.defaultView!=null&&document.defaultView.getComputedStyle!=null)w3=document.defaultView.getComputedStyle(h4,'').getPropertyValue("color");
this.a8.style.color=w3;
this.a8.colorSet=true;
}
this.a8.style.borderWidth="0px";
this.RestoreMargin(this.a8,false);
}
this.b1=0;
h4.insertBefore(this.a8,h4.firstChild);
this.SetEditorValue(this.a8,v9,e4);
if (this.a8.offsetHeight<h4.clientHeight&&this.a8.tagName!="TEXTAREA"){
if (h4.vAlign=="middle")
this.a8.style.posTop+=(h4.clientHeight-this.a8.offsetHeight)/2;
else if (h4.vAlign=="bottom")
this.a8.style.posTop+=(h4.clientHeight-this.a8.offsetHeight);
}
this.SizeAll(this.GetTopSpread(e4));
}
this.SetEditorFocus(this.a8);
if (e4.getAttribute("EditMode")=="replace"){
if ((this.a8.tagName=="INPUT"&&this.a8.type=="text")||this.a8.tagName=="TEXTAREA")
this.a8.select();
}
this.a7=true;
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null&&f9.disabled)
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Copy");
if (f9!=null&&!f9.disabled)
this.UpdateCmdBtnState(f9,true);
f9=this.GetCmdBtn(e4,"Paste");
if (f9!=null&&!f9.disabled)
this.UpdateCmdBtnState(f9,true);
f9=this.GetCmdBtn(e4,"Clear");
if (f9!=null&&!f9.disabled)
this.UpdateCmdBtnState(f9,true);
}
this.ScrollView(e4);
}
this.GetCurrency=function (validator){
var w4=validator.CurrencySymbol;
if (w4!=null)return w4;
var g0=document.getElementById(validator.id+"cs");
if (g0!=null){
return g0.innerHTML;
}
return "";
}
this.GetValueFromRender=function (e4,rd){
var j5=this.GetCellType2(this.GetCell(rd));
if (j5!=null){
if (j5=="text")j5="TextCellType";
var i3=null;
if (j5=="ExtenderCellType"){
i3=this.GetFunction(j5+"_getEditor")
if (i3!=null){
if (i3(rd)!=null)
i3=this.GetFunction(j5+"_getEditorValue");
else 
i3=null;
}
}else 
i3=this.GetFunction(j5+"_getValue");
if (i3!=null){
return i3(rd,e4);
}
}
var g0=rd;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
if (g0.innerHTML=="&nbsp;")return "";
var v9=g0.value;
if ((typeof(v9)=="undefined")&&j5=="readonly"&&g0.parentNode!=null&&g0.parentNode.getAttribute("CellType2")=="TagCloudCellType")
v9=g0.textContent;
if (v9==null){
v9=this.ReplaceAll(g0.innerHTML,"&nbsp;"," ");
v9=this.ReplaceAll(v9,"<br>"," ");
v9=this.HTMLDecode(v9);
}
return v9;
}
this.ReplaceAll=function (val,u1,dest){
if (val==null)return val;
var w5=val.length;
while (true){
val=val.replace(u1,dest);
if (val.length==w5)break ;
w5=val.length;
}
return val;
}
this.GetFormula=function (e4,h1,h3){
h1=this.GetDisplayIndex(e4,h1);
var h4=this.GetCellFromRowCol(e4,h1,h3);
var w6=this.GetFormulaFromCell(h4);
return w6;
}
this.SetFormula=function (e4,h1,h3,i3,recalc,clientOnly){
h1=this.GetDisplayIndex(e4,h1);
var h4=this.GetCellFromRowCol(e4,h1,h3);
h4.setAttribute("FpFormula",i3);
if (!clientOnly)
this.SetCellValue(e4,h4,i3,null,recalc);
}
this.GetFormulaFromCell=function (rd){
var v9=null;
if (rd.getAttribute("FpFormula")!=null){
v9=rd.getAttribute("FpFormula");
}
if (v9!=null)
v9=this.Trim(new String(v9));
return v9;
}
this.IsDouble=function (val,decimalchar,negsign,possign,minimumvalue,maximumvalue){
if (val==null||val.length==0)return true;
val=val.replace(" ","");
if (val.length==0)return true;
if (negsign!=null)val=val.replace(negsign,"-");
if (possign!=null)val=val.replace(possign,"+");
if (val.charAt(val.length-1)=="-")val="-"+val.substring(0,val.length-1);
var w7=new RegExp("^\\s*([-\\+])?(\\d+)?(\\"+decimalchar+"(\\d+))?([eE]([-\\+])?(\\d+))?\\s*$");
var w8=val.match(w7);
if (w8==null)
return false;
if ((w8[2]==null||w8[2].length==0)&&(w8[4]==null||w8[4].length==0))return false;
var w9="";
if (w8[1]!=null&&w8[1].length>0)w9=w8[1];
if (w8[2]!=null&&w8[2].length>0)
w9+=w8[2];
else 
w9+="0";
if (w8[4]!=null&&w8[4].length>0)
w9+=("."+w8[4]);
if (w8[6]!=null&&w8[6].length>0){
w9+=("E"+w8[6]);
if (w8[7]!=null)
w9+=(w8[7]);
else 
w9+="0";
}
var x0=parseFloat(w9);
if (isNaN(x0))return false;
var g0=true;
if (minimumvalue!=null){
var x1=parseFloat(minimumvalue);
g0=(!isNaN(x1)&&x0>=x1);
}
if (g0&&maximumvalue!=null){
var j1=parseFloat(maximumvalue);
g0=(!isNaN(j1)&&x0<=j1);
}
return g0;
}
this.GetFunction=function (fn){
if (fn==null||fn=="")return null;
try {
var g0=eval(fn);
return g0;
}catch (g3){
return null;
}
}
this.SetValueToRender=function (rd,val,valueonly){
var i3=null;
var j5=this.GetCellType2(this.GetCell(rd));
if (j5!=null){
if (j5=="text")j5="TextCellType";
if (j5=="ExtenderCellType"){
i3=this.GetFunction(j5+"_getEditor")
if (i3!=null){
if (i3(rd)!=null)
i3=this.GetFunction(j5+"_setEditorValue");
else 
i3=null;
}
}else 
i3=this.GetFunction(j5+"_setValue");
}
if (i3!=null){
i3(rd,val);
}else {
if (typeof(rd.value)!="undefined"){
if (val==null)val="";
rd.value=val;
}else {
var g0=rd;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
g0.innerHTML=this.ReplaceAll(val," ","&nbsp;");
}
}
if ((valueonly==null||!valueonly)&&rd.getAttribute("FpFormula")!=null){
rd.setAttribute("FpFormula",val);
}
}
this.Trim=function (t2){
var w8=t2.match(new RegExp("^\\s*(\\S+(\\s+\\S+)*)\\s*$"));
return (w8==null)?"":w8[1];
}
this.GetOffsetLeft=function (e4,h4,i8){
var e7=i8;
if (e7==null)e7=this.GetViewportFromCell(e4,h4);
var r6=0;
var g0=h4;
while (g0!=null&&g0!=e7&&this.IsChild(g0,e7)){
r6+=g0.offsetLeft;
g0=g0.offsetParent;
}
return r6;
}
this.GetOffsetTop=function (e4,h4,i8){
var e7=i8;
if (e7==null)e7=this.GetViewportFromCell(e4,h4);
var x2=0;
var g0=h4;
while (g0!=null&&g0!=e7&&this.IsChild(g0,e7)){
x2+=g0.offsetTop;
g0=g0.offsetParent;
}
return x2;
}
this.SetEditorFocus=function (g0){
if (g0==null)return ;
var x3=true;
var h4=this.GetCell(g0,true);
var j5=this.GetCellType(h4);
if (j5!=null){
var i3=this.GetFunction(j5+"_setFocus");
if (i3!=null){
i3(g0);
x3=false;
}
}
if (x3){
try {
g0.focus();
}catch (g3){}
}
}
this.SetEditorValue=function (g0,val,e4){
var h4=this.GetCell(g0,true);
var j5=this.GetCellType(h4);
if (j5!=null){
var i3=this.GetFunction(j5+"_setEditorValue");
if (i3!=null){
i3(g0,val,e4);
return ;
}
}
j5=g0.getAttribute("FpEditor");
if (j5!=null){
var i3=this.GetFunction(j5+"_setEditorValue");
if (i3!=null){
i3(g0,val,e4);
return ;
}
}
g0.value=val;
}
this.GetEditorValue=function (g0){
var h4=this.GetCell(g0,true);
var j5=this.GetCellType(h4);
if (j5!=null){
var i3=this.GetFunction(j5+"_getEditorValue");
if (i3!=null){
return i3(g0);
}
}
j5=g0.getAttribute("FpEditor");
if (j5!=null){
var i3=this.GetFunction(j5+"_getEditorValue");
if (i3!=null){
return i3(g0);
}
}
if (g0.type=="checkbox"){
if (g0.checked)
return "True";
else 
return "False";
}
else {
return g0.value;
}
}
this.CreateMsg=function (){
if (this.b2!=null)return ;
var g0=this.b2=document.createElement("div");
g0.style.position="absolute";
g0.style.background="yellow";
g0.style.color="red";
g0.style.border="1px solid black";
g0.style.display="none";
}
this.SetMsg=function (msg){
this.CreateMsg();
this.b2.innerHTML=msg;
this.b2.width=this.b2.offsetWidth+6;
}
this.ShowMsg=function (show){
this.CreateMsg();
if (show){
this.b2.style.display="block";
}
else 
this.b2.style.display="none";
}
this.EndEdit=function (){
if (this.a8!=null&&this.a8.parentNode!=null){
var h4=this.GetCell(this.a8.parentNode);
var e4=this.GetSpread(h4,false);
if (e4==null)return true;
var x4=this.GetEditorValue(this.a8);
var x5=x4;
if (typeof(x4)=="string")x5=this.Trim(x4);
var x6=(e4.getAttribute("AcceptFormula")=="true"&&x5!=null&&x5.charAt(0)=='=');
var i4=(this.b1!=0);
if (!x6&&!i4){
var x7=null;
var j5=this.GetCellType(h4);
if (j5!=null){
var i3=this.GetFunction(j5+"_isValid");
if (i3!=null){
x7=i3(h4,x4);
}
}
if (x7!=null&&x7!=""){
this.SetMsg(x7);
this.GetViewport(e4).parentNode.insertBefore(this.b2,this.GetViewport(e4).parentNode.firstChild);
this.ShowMsg(true);
this.SetValidatorPos(e4);
this.a8.focus();
return false;
}else {
this.ShowMsg(false);
}
}
if (!i4){
h4.removeChild(this.a8);
this.a8.style.display="none";
this.GetViewport(e4).parentNode.appendChild(this.a8);
this.SetEditorValue(this.a8,"",e4);
var x8=this.GetRender(h4);
if (x8.style.display=="none")x8.style.display="block";
if (this.b0!=null&&this.b0==x4){
this.SetValueToRender(x8,this.a9,true);
}else {
this.SetValueToRender(x8,x4);
}
this.RestoreMargin(h4);
}
if ((this.b0!=null&&this.b0!=x4)||(this.b0==null&&this.a9!=x4)){
this.SetCellValue(e4,h4,x4);
if (x4!=null&&x4.length>0&&x4.indexOf("=")==0)h4.setAttribute("FpFormula",x4);
}
if (!i4)
this.SizeAll(this.GetTopSpread(e4));
this.a8=null;
this.a7=false;
var g3=this.CreateEvent("EditStopped");
g3.cell=h4;
this.FireEvent(e4,g3);
this.Focus(e4);
var x9=e4.getAttribute("autoCalc");
if (x9!="false"){
if ((this.b0!=null&&this.b0!=x4)||(this.b0==null&&this.a9!=x4)){
this.UpdateValues(e4);
}
}
}
this.b1=-1;
return true;
}
this.SetValidatorPos=function (e4){
if (this.a8==null)return ;
var h4=this.GetCell(this.a8.parentNode);
if (h4==null)return ;
var g0=this.b2;
if (g0!=null&&g0.style.display!="none"){
var n5=this.GetViewport0(e4);
var f4=this.GetViewport1(e4);
var y0=this.GetViewport2(e4);
var e7=this.GetViewport(e4);
var q1=this.GetColHeader(e4).offsetHeight;
if ((n5!=null||f4!=null)&&(this.IsChild(h4,y0)||this.IsChild(h4,e7))){
if (n5!=null)
q1+=n5.offsetHeight;
else 
q1+=f4.offsetHeight;
}
var y1=this.GetRowHeader(e4).offsetWidth;
if ((n5!=null||y0!=null)&&(this.IsChild(h4,f4)||this.IsChild(h4,e7)))
{
if (n5!=null)
y1+=n5.offsetWidth;
else 
y1+=y0.offsetWidth;
}
if (e4.frzRows==0&&e4.frzCols==0){
q1=0;
y1=0;
}else {
if (e7!=null&&this.IsChild(h4,e7)){
q1-=e7.parentNode.scrollTop;
y1-=e7.parentNode.scrollLeft;
}
}
g0.style.left=""+(y1+h4.offsetLeft)+"px";
g0.style.top=""+(q1+h4.offsetTop+h4.offsetHeight)+"px";
if (h4.offsetTop+h4.offsetHeight+g0.offsetHeight+16>g0.parentNode.offsetHeight)
g0.style.top=""+(q1+h4.offsetTop-g0.offsetHeight-1)+"px";
}
}
this.SaveMargin=function (editCell){
if (editCell.style.paddingLeft!=null&&editCell.style.paddingLeft!=""){
this.b3.left=editCell.style.paddingLeft;
editCell.style.paddingLeft=0;
}
if (editCell.style.paddingRight!=null&&editCell.style.paddingRight!=""){
this.b3.right=editCell.style.paddingRight;
editCell.style.paddingRight=0;
}
if (editCell.style.paddingTop!=null&&editCell.style.paddingTop!=""){
this.b3.top=editCell.style.paddingTop;
editCell.style.paddingTop=0;
}
if (editCell.style.paddingBottom!=null&&editCell.style.paddingBottom!=""){
this.b3.bottom=editCell.style.paddingBottom;
editCell.style.paddingBottom=0;
}
}
this.RestoreMargin=function (h4,reset){
if (this.b3.left!=null&&this.b3.left!=-1){
h4.style.paddingLeft=this.b3.left;
if (reset==null||reset)this.b3.left=-1;
}
if (this.b3.right!=null&&this.b3.right!=-1){
h4.style.paddingRight=this.b3.right;
if (reset==null||reset)this.b3.right=-1;
}
if (this.b3.top!=null&&this.b3.top!=-1){
h4.style.paddingTop=this.b3.top;
if (reset==null||reset)this.b3.top=-1;
}
if (this.b3.bottom!=null&&this.b3.bottom!=-1){
h4.style.paddingBottom=this.b3.bottom;
if (reset==null||reset)this.b3.bottom=-1;
}
}
this.PaintSelectedCell=function (e4,h4,select,anchor){
if (h4==null)return ;
var y2=anchor?e4.getAttribute("anchorBackColor"):e4.getAttribute("selectedBackColor");
if (select){
if (h4.getAttribute("bgColorBak")==null)
h4.setAttribute("bgColorBak",document.defaultView.getComputedStyle(h4,"").getPropertyValue("background-color"));
if (h4.bgColor1==null)
h4.bgColor1=h4.style.backgroundColor;
h4.style.backgroundColor=y2;
if (h4.getAttribute("bgSelImg"))
h4.style.backgroundImage=h4.getAttribute("bgSelImg");
}else {
if (h4.bgColor1!=null)
h4.style.backgroundColor="";
if (h4.bgColor1!=null&&h4.bgColor1!="")
h4.style.backgroundColor=h4.bgColor1;
h4.style.backgroundImage="";
if (h4.getAttribute("bgImg")!=null)
h4.style.backgroundImage=h4.getAttribute("bgImg");
}
}
this.PaintAnchorCell=function (e4){
var o2=this.GetOperationMode(e4);
if (e4.d1==null||(o2!="Normal"&&o2!="RowMode"))return ;
if (o2=="MultiSelect"||o2=="ExtendedSelect")return ;
if (!this.IsChild(e4.d1,e4))return ;
var y3=(e4.frzRows==0&&e4.frzCols==0)&&(this.GetTopSpread(e4).getAttribute("hierView")!="true");
if (e4.getAttribute("showFocusRect")=="false")y3=false;
if (y3){
this.PaintSelectedCell(e4,e4.d1,false);
this.PaintFocusRect(e4);
this.PaintAnchorCellHeader(e4,true);
return ;
}
var g0=e4.d1.parentNode.cells[0].firstChild;
if (g0!=null&&g0.nodeName!="#text"&&g0.getAttribute("FpSpread")=="Spread")return ;
this.PaintSelectedCell(e4,e4.d1,true,true);
this.PaintAnchorCellHeader(e4,true);
}
this.ClearSelection=function (e4,thisonly){
var y4=this.GetParentSpread(e4);
if (thisonly==null&&y4!=null&&y4.getAttribute("hierView")=="true"){
this.ClearSelection(y4);
return ;
}
var i6=this.GetViewport(e4);
var f5=this.GetRowCount(e4);
if (i6!=null&&i6.rows.length>f5){
for (var f1=0;f1<i6.rows.length;f1++){
if (i6.rows[f1].cells.length>0&&i6.rows[f1].cells[0]!=null&&i6.rows[f1].cells[0].firstChild!=null&&i6.rows[f1].cells[0].firstChild.nodeName!="#text"){
var g0=i6.rows[f1].cells[0].firstChild;
if (g0.getAttribute("FpSpread")=="Spread"){
this.ClearSelection(g0,true);
}
}
}
}
this.DoclearSelection(e4);
if (e4.d1!=null){
var v8=this.GetOperationMode(e4);
if (v8=="RowMode"||v8=="SingleSelect"||v8=="ExtendedSelect"||v8=="MultiSelect"){
var h5=this.GetRowFromCell(e4,e4.d1);
this.PaintSelection(e4,h5,-1,1,-1,false);
}
this.PaintSelectedCell(e4,e4.d1,false);
this.PaintAnchorCellHeader(e4,false);
}else {
var h4=this.GetCellFromRowCol(e4,1,0);
if (h4!=null)this.PaintSelectedCell(e4,h4,false);
}
this.PaintFocusRect(e4);
e4.selectedCols=[];
e4.e2=true;
}
this.SetSelectedRange=function (e4,h1,h3,n6,h0,t3){
this.ClearSelection(e4);
var h1=this.GetDisplayIndex(e4,h1);
var u5=0;
var y5=n6;
var i6=this.GetViewport(e4);
if (i6!=null){
for (f1=h1;f1<i6.rows.length&&u5<y5;f1++){
if (this.IsChildSpreadRow(e4,i6,f1)){;
n6++;
}else {
u5++;
}
}
}
var y6=null;
var a2=null;
if (e4.getAttribute("LayoutMode")){
if (h3>=0&&n6<0){
if (h0!=1)return ;
var i9=this.GetDisplayIndex2(e4,t3);
var h4=this.GetCellByRowCol(e4,i9,h3);
if (h4!=null&&parseInt(h4.getAttribute("col"))!=-1){
h3=parseInt(h4.getAttribute("col"));
y6=parseInt(h4.parentNode.getAttribute("row"));
a2=this.GetColFromCell(e4,h4);
}
else 
return ;
this.PaintMultipleRowSelection(e4,y6,a2,1,h0,true);
}
else if (h1>=0&&h0<0){
if (n6>parseInt(e4.getAttribute("layoutrowcount")))return ;
var y7=parseInt(this.GetRowKeyFromRow(e4,h1));
var h1=parseInt(this.GetFirstRowFromKey(e4,y7));
this.UpdateAnchorCell(e4,h1,0,true);
n6=parseInt(e4.getAttribute("layoutrowcount"));
this.PaintSelection(h1,h3,n6,h0,true);
}
else if (h1>=0&&h3>=0&&(h0>1||n6>1))
return ;
}
else 
this.PaintSelection(e4,h1,h3,n6,h0,true);
this.SetSelection(e4,h1,h3,n6,h0,null,y6,a2);
}
this.AddSelection=function (e4,h1,h3,n6,h0,t3){
var h1=this.GetDisplayIndex(e4,h1);
var u5=0;
var y5=n6;
var i6=this.GetViewport(e4);
if (i6!=null){
for (f1=h1;f1<i6.rows.length&&u5<y5;f1++){
if (this.IsChildSpreadRow(e4,i6,f1)){;
n6++;
}else {
u5++;
}
}
}
var y6;
var a2;
if (e4.getAttribute("LayoutMode")){
if (h3>=0&&n6<0){
if (h0!=1)return ;
var i9=this.GetDisplayIndex2(e4,t3);
var h4=this.GetCellByRowCol(e4,i9,h3);
if (h4!=null&&parseInt(h4.getAttribute("col"))!=-1){
y6=parseInt(h4.parentNode.getAttribute("row"));
a2=this.GetColFromCell(e4,h4);
h3=parseInt(h4.getAttribute("col"));
}
else 
return ;
this.PaintMultipleRowSelection(e4,y6,a2,1,h0,true);
}
else if (h1>=0&&h0<0){
if (n6>parseInt(e4.getAttribute("layoutrowcount")))return ;
var y7=parseInt(this.GetRowKeyFromRow(e4,h1));
var h1=parseInt(this.GetFirstRowFromKey(e4,y7));
if (e4.allowGroup){
this.ClearSelection(e4);
this.UpdateAnchorCell(e4,h1,0,true);
}
n6=parseInt(e4.getAttribute("layoutrowcount"));
this.PaintSelection(e4,h1,h3,n6,h0,true);
}
else if (h1>=0&&h3>=0&&(h0>1||n6>1))
return ;
}
else 
this.PaintSelection(e4,h1,h3,n6,h0,true);
this.SetSelection(e4,h1,h3,n6,h0,true,y6,a2);
}
this.SelectRow=function (e4,index,u5,select,ignoreAnchor){
e4.d5=index;
e4.d6=-1;
if (!ignoreAnchor)this.UpdateAnchorCell(e4,index,0,false);
e4.d7="r";
var y8=u5;
if (e4.getAttribute("LayoutMode")){
y8=parseInt(e4.getAttribute("layoutrowcount"));
}
this.PaintSelection(e4,index,-1,y8,-1,select);
if (select)
{
this.SetSelection(e4,index,-1,u5,-1);
}else {
this.RemoveSelection(e4,index,-1,u5,-1);
this.PaintFocusRect(e4);
}
}
this.SelectColumn=function (e4,index,u5,select,ignoreAnchor){
e4.d5=-1;
e4.d6=index;
if (!ignoreAnchor){
var h1=0;
var y9=index;
if (e4.getAttribute("LayoutMode")){
var h4=e4.d1;
if (parseInt(e4.d1.getAttribute("col"))==-1)return ;
if (h4){
h1=this.GetRowFromCell(e4,h4);
y9=this.GetColFromCell(e4,h4);
}
e4.copymulticol=true;
}
this.UpdateAnchorCell(e4,h1,y9,false,true);
}
e4.d7="c";
if (!e4.getAttribute("LayoutMode"))
this.PaintSelection(e4,-1,y9,-1,u5,select);
else 
this.PaintMultipleRowSelection(e4,h1,y9,1,u5,select);
if (select)
{
this.SetSelection(e4,-1,index,-1,u5,null,h1,y9);
this.AddColSelection(e4,index);
}
}
this.AddColSelection=function (e4,index){
var z0=0;
for (var f1=0;f1<e4.selectedCols.length;f1++){
if (e4.selectedCols[f1]==index)return ;
if (index>e4.selectedCols[f1])z0=f1+1;
}
e4.selectedCols.length++;
for (var f1=e4.selectedCols.length-1;f1>z0;f1--)
e4.selectedCols[f1]=e4.selectedCols[f1-1];
e4.selectedCols[z0]=index;
}
this.IsColSelected=function (e4,u6){
for (var f1=0;f1<e4.selectedCols.length;f1++)
if (e4.selectedCols[f1]==u6)return true;
return false;
}
this.SyncColSelection=function (e4){
e4.selectedCols=[];
var z1=this.GetSelectedRanges(e4);
for (var f1=0;f1<z1.length;f1++){
var h9=z1[f1];
if (h9.type=="Column"){
for (var i1=h9.col;i1<h9.col+h9.colCount;i1++){
this.AddColSelection(e4,i1);
}
}
}
}
this.InitMovingCol=function (e4,u6,isGroupBar,p1){
if (e4.getAttribute("LayoutMode")&&u6==-1)return ;
if (this.GetOperationMode(e4)!="Normal"){
e4.selectedCols=[];
e4.selectedCols.push(u6);
}
if (isGroupBar){
this.dragCol=u6;
this.dragViewCol=this.GetColByKey(e4,u6);
}else {
if (e4.getAttribute("LayoutMode"))
this.dragCol=this.GetColTemplateRowFromGroupCell(e4,u6);
else 
this.dragCol=parseInt(this.GetSheetColIndex(e4,u6));
this.dragViewCol=u6;
}
var z2=this.GetMovingCol(e4);
if (isGroupBar){
this.ClearSelection(e4);
z2.innerHTML="";
var z3=document.createElement("DIV");
z3.innerHTML=p1.innerHTML;
z3.style.borderTop="0px solid";
z3.style.borderLeft="0px solid";
z3.style.borderRight="#808080 1px solid";
z3.style.borderBottom="#808080 1px solid";
z3.style.width=""+Math.max(this.GetPreferredCellWidth(e4,p1),80)+"px";
z2.appendChild(z3);
if (e4.getAttribute("DragColumnCssClass")==null){
z2.style.backgroundColor=p1.style.backgroundColor;
z2.style.paddingTop="1px";
z2.style.paddingBottom="1px";
}
z2.style.top="-50px";
z2.style.left="-100px";
}else {
var z4=0;
z2.style.top="0px";
z2.style.left="-1000px";
z2.style.display="";
z2.innerHTML="";
var z5=document.createElement("TABLE");
z2.appendChild(z5);
var i9=document.createElement("TR");
z5.appendChild(i9);
for (var f1=0;f1<e4.selectedCols.length;f1++){
var h4=document.createElement("TD");
i9.appendChild(h4);
var z6;
var z7;
if (e4.getAttribute("LayoutMode")){
z6=this.GetRowTemplateRowFromGroupCell(e4,u6,true);
z7=this.GetColTemplateRowFromGroupCell(e4,u6);
}
else {
if (e4.getAttribute("columnHeaderAutoTextIndex")!=null)
z6=parseInt(e4.getAttribute("columnHeaderAutoTextIndex"));
else 
z6=e4.getAttribute("ColHeaders")-1;
z7=e4.selectedCols[f1];
}
var z8=this.GetHeaderCellFromRowCol(e4,z6,z7,true);
if (z8.getAttribute("FpCellType")=="ExtenderCellType"&&z8.getElementsByTagName("DIV").length>0){
var z9=this.GetEditor(z8);
var aa0=this.GetFunction("ExtenderCellType_getEditorValue");
if (z9!==null&&aa0!==null){
h4.innerHTML=aa0(z9);
}
}
else 
h4.innerHTML=z8.innerHTML;
h4.style.cssText=z8.style.cssText;
h4.style.borderTop="0px solid";
h4.style.borderLeft="0px solid";
h4.style.borderRight="#808080 1px solid";
h4.style.borderBottom="#808080 1px solid";
h4.setAttribute("align","center");
var j3=Math.max(this.GetPreferredCellWidth(e4,z8),80);
h4.style.width=""+j3+"px";
z4+=j3;
}
if (e4.getAttribute("DragColumnCssClass")==null){
z2.style.backgroundColor=e4.getAttribute("SelectedBackColor");
z2.style.tableLayout="fixed";
z2.style.width=""+z4+"px";
}
}
e4.selectedCols.context=[];
var aa1=e4.selectedCols.context;
var r6=0;
var f3=this.GetColGroup(this.GetFrozColHeader(e4));
if (f3!=null){
for (var f1=0;f1<f3.childNodes.length;f1++){
var aa2=f3.childNodes[f1].offsetWidth;
aa1.push({left:r6,width:aa2});
r6+=aa2;
}
}
f3=this.GetColGroup(this.GetColHeader(e4));
if (f3!=null){
for (var f1=0;f1<f3.childNodes.length;f1++){
var aa2=f3.childNodes[f1].offsetWidth;
aa1.push({left:r6,width:aa2});
r6+=aa2;
}
}
}
this.SelectTable=function (e4,select){
if (select)this.UpdateAnchorCell(e4,0,0,false);
e4.d7="t";
this.PaintSelection(e4,-1,-1,-1,-1,select);
if (select)
{
this.SetSelection(e4,-1,-1,-1,-1);
}
}
this.GetSpanCell=function (h1,h3,span){
if (span==null){
return null;
}
var u5=span.length;
for (var f1=0;f1<u5;f1++){
var r8=span[f1];
var aa3=(r8.row<=h1&&h1<r8.row+r8.rowCount&&r8.col<=h3&&h3<r8.col+r8.colCount);
if (aa3)return r8;
}
return null;
}
this.IsCovered=function (e4,h1,h3,span){
var r8=this.GetSpanCell(h1,h3,span);
if (r8==null){
return false;
}else {
if (r8.row==h1&&r8.col==h3)return false;
return true;
}
}
this.IsSpanCell=function (e4,h1,h3){
var d9=e4.d9;
var u5=d9.length;
for (var f1=0;f1<u5;f1++){
var r8=d9[f1];
var aa3=(r8.row==h1&&r8.col==h3);
if (aa3)return r8;
}
return null;
}
this.SelectRange=function (e4,h1,h3,n6,h0,select){
e4.d7="";
this.UpdateRangeSelection(e4,h1,h3,n6,h0,select);
if (select){
this.SetSelection(e4,h1,h3,n6,h0);
this.PaintAnchorCell(e4);
}
}
this.UpdateRangeSelection=function (e4,h1,h3,n6,h0,select){
var i6=this.GetViewport(e4);
this.UpdateRangeSelection(e4,h1,h3,n6,h0,select,i6);
}
this.GetSpanCells=function (e4,i6){
if (i6==this.GetViewport(e4)||i6==this.GetViewport1(e4)||i6==this.GetViewport2(e4)||i6==this.GetViewport0(e4))
return e4.d9;
else if (i6==this.GetColHeader(e4)||i6==this.GetFrozColHeader(e4))
return e4.e1;
else if (i6==this.GetRowHeader(e4)||i6==this.GetFrozRowHeader(e4))
return e4.e0;
return null;
}
this.UpdateRangeSelection=function (e4,h1,h3,n6,h0,select,i6){
if (i6==null)return ;
for (var f1=h1;f1<h1+n6&&f1<i6.rows.length;f1++){
if (this.IsChildSpreadRow(e4,i6,f1))continue ;
var aa4=this.GetCellIndex(e4,f1,h3,this.GetSpanCells(e4,i6));
for (var i1=0;i1<h0;i1++){
if (this.IsCovered(e4,f1,h3+i1,this.GetSpanCells(e4,i6)))continue ;
if (aa4<i6.rows[f1].cells.length){
this.PaintSelectedCell(e4,i6.rows[f1].cells[aa4],select);
}
aa4++;
}
}
}
this.GetColFromCell=function (e4,h4){
if (h4==null)return -1;
var h1=this.GetRowFromCell(e4,h4);
return this.GetColIndex(e4,h1,h4.cellIndex,this.GetSpanCells(e4,h4.parentNode.parentNode.parentNode),this.InFrozCols(e4,h4),this.IsChild(h4,this.GetFrozRowHeader(e4))||this.IsChild(h4,this.GetRowHeader(e4)));
}
this.GetRowFromCell=function (e4,h4){
if (h4==null||h4.parentNode==null)return -1;
var h1=h4.parentNode.rowIndex;
if (e4.frzRows>0&&(this.IsChild(h4,this.GetViewport2(e4))||this.IsChild(h4,this.GetViewport(e4))||this.IsChild(h4,this.GetRowHeader(e4)))){
h1+=e4.frzRows;
}
return h1;
}
this.GetColIndex=function (e4,f1,t9,span,frozArea,c5){
var aa5=0;
var u5=this.GetColCount(e4);
var aa6=e4.frzCols;
if (frozArea){
u5=e4.frzCols;
aa6=0;
}else if (c5){
aa6=0;
var f3=null;
if (this.GetFrozRowHeader(e4)!=null)
f3=this.GetColGroup(this.GetFrozRowHeader(e4));
else if (this.GetRowHeader(e4)!=null)
f3=this.GetColGroup(this.GetRowHeader(e4));
if (f3!=null)
u5=f3.childNodes.length;
}
for (var i1=aa6;i1<u5;i1++){
if (this.IsCovered(e4,f1,i1,span))continue ;
if (aa5==t9){
return i1;
}
aa5++;
}
return u5;
}
this.GetCellIndex=function (e4,f1,u6,span){
var aa7=false;
var e7=this.GetViewport(e4);
if (e7!=null)aa7=e7.parentNode.getAttribute("hiddenCells");
if (aa7&&span==e4.d9){
if (span!=e4.e0&&u6>=e4.frzCols){
return u6-e4.frzCols;
}
return u6;
}else {
var aa6=0;
var u5=u6;
if (span!=e4.e0&&u6>=e4.frzCols){
aa6=e4.frzCols;
u5=u6-e4.frzCols;
}
var aa5=0;
for (var i1=0;i1<u5;i1++){
if (this.IsCovered(e4,f1,aa6+i1,span))continue ;
aa5++;
}
return aa5;
}
}
this.NextCell=function (e4,event,key){
if (event.altKey)return ;
var aa8=this.GetParent(this.GetViewport(e4));
if (e4.d1==null){
var i3=this.FireActiveCellChangingEvent(e4,0,0);
if (!i3){
e4.SetActiveCell(0,0);
var g3=this.CreateEvent("ActiveCellChanged");
g3.cmdID=e4.id;
g3.row=g3.Row=0;
g3.col=g3.Col=0;
this.FireEvent(e4,g3);
}
return ;
}
if (event.shiftKey&&key!=event.DOM_VK_TAB){
this.CancelDefault(event);
var r4=this.GetOperationMode(e4);
if (r4=="RowMode"||r4=="SingleSelect"||r4=="MultiSelect"||(r4=="Normal"&&this.GetSelectionPolicy(e4)=="Single"))return ;
var r8=this.GetSpanCell(e4.d3,e4.d4,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
switch (key){
case event.DOM_VK_RIGHT:
var h1=e4.d3;
var h3=e4.d4+1;
if (r8!=null){
h3=r8.col+r8.colCount;
}
if (h3>this.GetColCount(e4)-1)return ;
e4.d4=h3;
e4.d2=this.GetCellFromRowCol(e4,h1,h3);
this.Select(e4,e4.d1,e4.d2);
break ;
case event.DOM_VK_LEFT:
var h1=e4.d3;
var h3=e4.d4-1;
if (r8!=null){
h3=r8.col-1;
}
r8=this.GetSpanCell(h1,h3,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (r8!=null){
if (this.IsSpanCell(e4,h1,r8.col))h3=r8.col;
}
if (h3<0)return ;
e4.d4=h3;
e4.d2=this.GetCellFromRowCol(e4,h1,h3);
this.Select(e4,e4.d1,e4.d2);
break ;
case event.DOM_VK_DOWN:
var h1=e4.d3+1;
var h3=e4.d4;
if (r8!=null){
h1=r8.row+r8.rowCount;
}
h1=this.GetNextRow(e4,h1);
if (h1>this.GetRowCountInternal(e4)-1)return ;
e4.d3=h1;
e4.d2=this.GetCellFromRowCol(e4,h1,h3);
this.Select(e4,e4.d1,e4.d2);
break ;
case event.DOM_VK_UP:
var h1=e4.d3-1;
var h3=e4.d4;
if (r8!=null){
h1=r8.row-1;
}
h1=this.GetPrevRow(e4,h1);
r8=this.GetSpanCell(h1,h3,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (r8!=null){
if (this.IsSpanCell(e4,r8.row,h3))h1=r8.row;
}
if (h1<0)return ;
e4.d3=h1;
e4.d2=this.GetCellFromRowCol(e4,h1,h3);
this.Select(e4,e4.d1,e4.d2);
break ;
case event.DOM_VK_HOME:
if (e4.d1.parentNode.rowIndex>=0){
e4.d4=0;
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
break ;
case event.DOM_VK_END:
if (e4.d1.parentNode.rowIndex>=0){
e4.d4=this.GetColCount(e4)-1;
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
break ;
case event.DOM_VK_PAGE_DOWN:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>=0){
h1=0;
for (h1=0;h1<this.GetViewport(e4).rows.length;h1++){
if (this.GetViewport(e4).rows[h1].offsetTop+this.GetViewport(e4).rows[h1].offsetHeight>this.GetViewport(e4).parentNode.offsetHeight+this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
h1=this.GetNextRow(e4,h1);
if (h1<this.GetViewport(e4).rows.length){
this.GetViewport(e4).parentNode.scrollTop=this.GetViewport(e4).rows[h1].offsetTop;
e4.d3=h1;
}else {
h1=this.GetRowCountInternal(e4)-1;
e4.d3=h1;
}
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
break ;
case event.DOM_VK_PAGE_UP:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>0){
h1=0;
for (h1=0;h1<this.GetViewport(e4).rows.length;h1++){
if (this.GetViewport(e4).rows[h1].offsetTop+this.GetViewport(e4).rows[h1].offsetHeight>this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
if (h1<this.GetViewport(e4).rows.length){
var k0=0;
while (h1>0){
k0+=this.GetViewport(e4).rows[h1].offsetHeight;
if (k0>this.GetViewport(e4).parentNode.offsetHeight){
break ;
}
h1--;
}
h1=this.GetPrevRow(e4,h1);
if (h1>=0){
this.GetViewport(e4).parentNode.scrollTop=this.GetViewport(e4).rows[h1].offsetTop;
e4.d3=h1;
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
}
}
break ;
}
this.SyncColSelection(e4);
}else {
var aa9=(key==event.DOM_VK_TAB);
if (key==event.DOM_VK_TAB){
if (event.shiftKey)key=event.DOM_VK_LEFT;
else key=event.DOM_VK_RIGHT;
}
var ab0=e4.d1;
var h1=e4.d3;
var h3=e4.d4;
switch (key){
case event.DOM_VK_RIGHT:
if (event.keyCode==event.DOM_VK_TAB){
var ab1=h1;
var ab2=h3;
do {
this.MoveRight(e4,h1,h3);
h1=e4.d3;
h3=e4.d4;
}while (!(ab1==h1&&ab2==h3)&&this.GetCellFromRowCol(e4,h1,h3).getAttribute("TabStop")!=null&&this.GetCellFromRowCol(e4,h1,h3).getAttribute("TabStop")=="false")
}
else {
this.MoveRight(e4,h1,h3);
}
break ;
case event.DOM_VK_LEFT:
if (event.keyCode==event.DOM_VK_TAB){
var ab1=h1;
var ab2=h3;
do {
this.MoveLeft(e4,h1,h3);
h1=e4.d3;
h3=e4.d4;
}while (!(ab1==h1&&ab2==h3)&&this.GetCellFromRowCol(e4,h1,h3).getAttribute("TabStop")!=null&&this.GetCellFromRowCol(e4,h1,h3).getAttribute("TabStop")=="false")
}
else {
this.MoveLeft(e4,h1,h3);
}
break ;
case event.DOM_VK_DOWN:
this.MoveDown(e4,h1,h3);
break ;
case event.DOM_VK_UP:
this.MoveUp(e4,h1,h3);
break ;
case event.DOM_VK_HOME:
if (e4.d1.parentNode.rowIndex>=0){
this.UpdateLeadingCell(e4,h1,0);
}
break ;
case event.DOM_VK_END:
if (e4.d1.parentNode.rowIndex>=0){
h3=this.GetColCount(e4)-1;
this.UpdateLeadingCell(e4,h1,h3);
}
break ;
case event.DOM_VK_PAGE_DOWN:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>=0){
h1=0;
for (h1=0;h1<this.GetViewport(e4).rows.length;h1++){
if (this.GetViewport(e4).rows[h1].offsetTop+this.GetViewport(e4).rows[h1].offsetHeight>this.GetViewport(e4).parentNode.offsetHeight+this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
h1=this.GetNextRow(e4,h1);
if (h1<this.GetViewport(e4).rows.length){
var g0=this.GetViewport(e4).rows[h1].offsetTop;
this.UpdateLeadingCell(e4,h1,e4.d4);
this.GetViewport(e4).parentNode.scrollTop=g0;
}else {
h1=this.GetPrevRow(e4,this.GetRowCount(e4)-1);
this.UpdateLeadingCell(e4,h1,e4.d4);
}
}
break ;
case event.DOM_VK_PAGE_UP:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>=0){
h1=0;
for (h1=0;h1<this.GetViewport(e4).rows.length;h1++){
if (this.GetViewport(e4).rows[h1].offsetTop+this.GetViewport(e4).rows[h1].offsetHeight>this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
if (h1<this.GetViewport(e4).rows.length){
var k0=0;
while (h1>=0){
k0+=this.GetViewport(e4).rows[h1].offsetHeight;
if (k0>this.GetViewport(e4).parentNode.offsetHeight){
break ;
}
h1--;
}
h1=this.GetPrevRow(e4,h1);
if (h1>=0){
var g0=this.GetViewport(e4).rows[h1].offsetTop;
this.UpdateLeadingCell(e4,h1,e4.d4);
this.GetViewport(e4).parentNode.scrollTop=g0;
}
}
}
break ;
}
if (ab0!=e4.d1){
var g3=this.CreateEvent("ActiveCellChanged");
g3.cmdID=e4.id;
g3.Row=g3.row=this.GetSheetIndex(e4,this.GetRowFromCell(e4,e4.d1));
g3.Col=g3.col=this.GetColFromCell(e4,e4.d1);
if (e4.getAttribute("LayoutMode"))
g3.InnerRow=g3.innerRow=e4.d1.parentNode.getAttribute("row");
this.FireEvent(e4,g3);
}
}
var h4=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
if (key==event.DOM_VK_LEFT&&h4.offsetLeft<aa8.scrollLeft){
if (h4.cellIndex>0)
aa8.scrollLeft=e4.d1.offsetLeft;
else 
aa8.scrollLeft=0;
}else if (h4.cellIndex==0){
aa8.scrollLeft=0;
}
if (key==event.DOM_VK_RIGHT&&h4.offsetLeft+h4.offsetWidth>aa8.scrollLeft+aa8.offsetWidth-10){
aa8.scrollLeft+=h4.offsetWidth;
}
if (key==event.DOM_VK_UP&&h4.parentNode.offsetTop<aa8.scrollTop){
if (h4.parentNode.rowIndex>1)
aa8.scrollTop=h4.parentNode.offsetTop;
else 
aa8.scrollTop=0;
}else if (h4.parentNode.rowIndex==1){
aa8.scrollTop=0;
}
var ab3=this.GetParent(this.GetViewport(e4));
aa8=this.GetParent(this.GetViewport(e4));
if (key==event.DOM_VK_DOWN&&(this.IsChild(h4,aa8)||this.IsChild(h4,this.GetViewport2(e4)))&&h4.offsetTop+h4.offsetHeight>aa8.scrollTop+aa8.clientHeight){
ab3.scrollTop+=h4.offsetHeight;
}
if (h4!=null&&h4.offsetWidth<aa8.clientWidth){
if ((this.IsChild(h4,aa8)||this.IsChild(h4,this.GetViewport1(e4)))&&h4.offsetLeft+h4.offsetWidth>aa8.scrollLeft+aa8.clientWidth){
ab3.scrollLeft=h4.offsetLeft+h4.offsetWidth-aa8.clientWidth;
}
}
if ((this.IsChild(h4,aa8)||this.IsChild(h4,this.GetViewport1(e4)))&&h4.offsetTop+h4.offsetHeight>aa8.scrollTop+aa8.clientHeight&&h4.offsetHeight<aa8.clientHeight){
ab3.scrollTop=h4.offsetTop+h4.offsetHeight-aa8.clientHeight;
}
if (h4.offsetTop<aa8.scrollTop){
ab3.scrollTop=h4.offsetTop;
}
this.ScrollView(e4);
this.EnableButtons(e4);
this.SaveData(e4);
var i3=true;
if (e4.d1!=null){
var i4=this.GetEditor(e4.d1);
if (i4!=null){
if (event.shiftKey&&!aa9){
i4.blur();
}else {
this.SetEditorFocus(i4);
if (!i4.disabled&&(i4.type==null||i4.type=="checkbox"||i4.type=="radio"||i4.type=="text"||i4.type=="password"||i4.tagName=="SELECT")){
this.a7=true;
this.a8=i4;
this.a9=this.GetEditorValue(i4);
}
}
i3=false;
}else {
this.a7=false;
this.a8=null;
}
}
if (i3)this.Focus(e4);
}
this.MoveUp=function (e4,h1,h3){
var n6=this.GetRowCountInternal(e4);
var h0=this.GetColCount(e4);
h1--;
h1=this.GetPrevRow(e4,h1);
if (h1>=0){
e4.d3=h1;
this.UpdateLeadingCell(e4,e4.d3,e4.d4);
}
}
this.MoveDown=function (e4,h1,h3){
var n6=this.GetRowCountInternal(e4);
var h0=this.GetColCount(e4);
var r8=this.GetSpanCell(h1,h3,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (r8!=null){
h1=r8.row+r8.rowCount;
}else {
h1++;
}
h1=this.GetNextRow(e4,h1);
if (h1==n6)h1=n6-1;
if (h1<n6){
e4.d3=h1;
this.UpdateLeadingCell(e4,e4.d3,e4.d4);
}
}
this.MoveLeft=function (e4,h1,h3){
var ab4=h1;
var n6=this.GetRowCountInternal(e4);
var h0=this.GetColCount(e4);
var r8=this.GetSpanCell(h1,h3,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (r8!=null){
h3=r8.col-1;
}else {
h3--;
}
if (h3<0){
h3=h0-1;
h1--;
if (h1<0){
h1=n6-1;
}
h1=this.GetPrevRow(e4,h1);
if (h1<0){
h1=n6-1;
}
h1=this.GetPrevRow(e4,h1);
e4.d3=h1;
}
var ab5=this.UpdateLeadingCell(e4,e4.d3,h3);
if (ab5)e4.d3=ab4;
}
this.MoveRight=function (e4,h1,h3){
var ab4=h1;
var n6=this.GetRowCountInternal(e4);
var h0=this.GetColCount(e4);
var r8=this.GetSpanCell(h1,h3,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (r8!=null){
h3=r8.col+r8.colCount;
}else {
h3++;
}
if (h3>=h0){
h3=0;
h1++;
if (h1>=n6)h1=0;
h1=this.GetNextRow(e4,h1);
if (h1>=n6)h1=0;
h1=this.GetNextRow(e4,h1);
e4.d3=h1;
}
var ab5=this.UpdateLeadingCell(e4,e4.d3,h3);
if (ab5)e4.d3=ab4;
}
this.UpdateLeadingCell=function (e4,h1,h3){
var ab6=0;
if (e4.getAttribute("LayoutMode")){
ab6=this.GetRowFromViewPort(e4,h1,h3);
var ab7=this.GetCellFromRowCol(e4,ab6,h3);
if (ab7)ab6=ab7.parentNode.getAttribute("row");
}
var i3=this.FireActiveCellChangingEvent(e4,h1,h3,ab6);
if (!i3){
var o2=this.GetOperationMode(e4);
if (o2!="MultiSelect")
this.ClearSelection(e4);
e4.d4=h3;
e4.d3=h1;
e4.d6=h3;
e4.d5=h1;
this.UpdateAnchorCell(e4,h1,h3);
}
return i3;
}
this.GetPrevRow=function (e4,h1){
if (h1<0)return 0;
var i6=this.GetViewport(e4);
if (h1<e4.frzRows){
i6=this.GetViewport0(e4);
if (i6==null)i6=this.GetViewport1(e4);
}
while (i6!=null&&h1<i6.rows.length){
if (this.IsChildSpreadRow(e4,i6,h1))
h1--;
else 
break ;
}
var h7=0;
if (h1>=e4.frzRows){
h1=h1-e4.frzRows;
h7=e4.frzRows;
}
if (e4.frzCols<this.GetColCount(e4)){
var y0=this.GetViewport2(e4);
while ((y0==null||h1<y0.rows.length&&y0.rows[h1].cells.length==0)&&i6!=null&&h1>0&&i6.rows[h1].cells.length==0)h1--;
}
if (i6!=null&&h1>=0&&h1<i6.rows.length){
if (i6.rows[h1].getAttribute("previewrow")){
h1--;
}
}
return h1+h7;
}
this.GetNextRow=function (e4,h1){
var i6=this.GetViewport(e4);
if (h1<e4.frzRows){
i6=this.GetViewport0(e4);
if (i6==null)i6=this.GetViewport1(e4);
}
while (i6!=null&&h1<i6.rows.length){
if (this.IsChildSpreadRow(e4,i6,h1))h1++;
else 
break ;
}
var h7=0;
if (h1>=e4.frzRows){
h1=h1-e4.frzRows;
h7=e4.frzRows;
}
if (e4.frzCols<this.GetColCount(e4)){
var y0=this.GetViewport2(e4);
while ((y0==null||h1<y0.rows.length&&y0.rows[h1].cells.length==0)&&i6!=null&&h1<i6.rows.length&&i6.rows[h1].cells.length==0)h1++;
}
if (i6!=null&&h1>=0&&h1<i6.rows.length){
if (i6.rows[h1].getAttribute("previewrow")){
h1++;
}
}
return h1+h7;
}
this.FireActiveCellChangingEvent=function (e4,i9,n9,t3){
var g3=this.CreateEvent("ActiveCellChanging");
g3.cancel=false;
g3.cmdID=e4.id;
g3.row=this.GetSheetIndex(e4,i9);
g3.col=n9;
if (e4.getAttribute("LayoutMode"))
g3.innerRow=t3;
this.FireEvent(e4,g3);
return g3.cancel;
}
this.GetSheetRowIndex=function (e4,h1){
h1=this.GetDisplayIndex(e4,h1);
if (h1<0)return -1;
var m9=null;
if (e4.frzRows>0){
if (h1>=e4.frzRows&&this.GetViewport(e4)!=null){
m9=this.GetViewport(e4).rows[h1-e4.frzRows];
}else if (h1<e4.frzRows&&this.GetViewport1(e4)!=null){
m9=this.GetViewport1(e4).rows[h1];
}
}else {
m9=this.GetViewport(e4).rows[h1];
}
if (m9!=null){
return m9.getAttribute("FpKey");
}else {
return -1;
}
}
this.GetSheetColIndex=function (e4,h3,t3){
var n9=-1;
if (e4.getAttribute("LayoutMode")){
var h1=this.GetDisplayIndex2(e4,t3);
var h4=this.GetCellByRowCol(e4,h1,h3);
if (h4!=null)
n9=parseInt(h4.getAttribute("col"));
}
else {
var f3=null;
if (e4.frzCols>0&&h3<e4.frzCols){
var k8=this.GetFrozColHeader(e4);
if (k8!=null&&k8.rows.length>0){
f3=this.GetColGroup(k8);
}else {
var m3=this.GetViewport2(e4);
if (m3!=null&&m3.rows.length>0){
f3=this.GetColGroup(m3);
}else {
var m1=this.GetViewport0(e4);
if (m1!=null&&m1.rows.length>0){
f3=this.GetColGroup(m1);
}
}
}
if (f3!=null&&h3>=0&&h3<f3.childNodes.length)n9=f3.childNodes[h3].getAttribute("FpCol");
}else {
var ab8=this.GetColHeader(e4);
if (ab8!=null&&ab8.rows.length>0){
f3=this.GetColGroup(ab8);
}else {
var e7=this.GetViewport(e4);
if (e7!=null&&e7.rows.length>0){
f3=this.GetColGroup(e7);
}else {
var m2=this.GetViewport1(e4);
if (m2!=null&&m2.rows.length>0){
f3=this.GetColGroup(m2);
}
}
}
if (f3!=null&&h3-e4.frzCols>=0&&h3-e4.frzCols<f3.childNodes.length){
n9=f3.childNodes[h3-e4.frzCols].getAttribute("FpCol");
}
}
}
return n9;
}
this.GetCellByRowCol=function (e4,h1,h3){
h1=this.GetDisplayIndex(e4,h1);
return this.GetCellFromRowCol(e4,h1,h3);
}
this.GetHeaderCellFromRowCol=function (e4,h1,h3,c6){
if (h1<0||h3<0)return null;
var e7=null;
if (c6){
if (h3<e4.frzCols){
e7=this.GetFrozColHeader(e4);
}else {
e7=this.GetColHeader(e4);
}
}else {
if (h1<e4.frzRows){
e7=this.GetFrozRowHeader(e4);
}else {
e7=this.GetRowHeader(e4);
}
}
var r8=this.GetSpanCell(h1,h3,this.GetSpanCells(e4,e7));
if (r8!=null){
h1=r8.row;
h3=r8.col;
}
var t8=this.GetCellIndex(e4,h1,h3,this.GetSpanCells(e4,e7));
if (!c6){
if (h1>=e4.frzRows){
h1-=e4.frzRows;
}
}
return e7.rows[h1].cells[t8];
}
this.GetCellFromRowCol=function (e4,h1,h3,prevCell){
if (h1<0||h3<0)return null;
var e7=null;
if (h1<e4.frzRows){
if (h3<e4.frzCols){
e7=this.GetViewport0(e4);
}else {
e7=this.GetViewport1(e4);
}
}else {
if (h3<e4.frzCols){
e7=this.GetViewport2(e4);
}else {
e7=this.GetViewport(e4);
}
}
var d9=e4.d9;
var r8=this.GetSpanCell(h1,h3,d9);
if (r8!=null){
h1=r8.row;
h3=r8.col;
}
var t8=0;
var aa7=false;
if (e7!=null)aa7=e7.parentNode.getAttribute("hiddenCells");
if (prevCell!=null&&!aa7){
if (prevCell.cellIndex<prevCell.parentNode.cells.length-1)
t8=prevCell.cellIndex+1;
}
else 
{
t8=this.GetCellIndex(e4,h1,h3,d9);
}
if (h1>=e4.frzRows){
h1-=e4.frzRows;
}
if (h1>=0&&h1<e7.rows.length)
return e7.rows[h1].cells[t8];
else 
return null;
}
this.GetHiddenValue=function (e4,h1,colName){
if (colName==null)return ;
h1=this.GetDisplayIndex(e4,h1);
var v9=null;
var e7=null;
e7=this.GetViewport(e4);
if (e7!=null&&h1>=0&&h1<e7.rows.length){
var m9=e7.rows[h1];
v9=m9.getAttribute("hv"+colName);
}
return v9;
}
this.GetValue=function (e4,h1,h3){
h1=this.GetDisplayIndex(e4,h1);
var h4=this.GetCellFromRowCol(e4,h1,h3);
var j4=this.GetRender(h4);
var v9=this.GetValueFromRender(e4,j4);
if (v9!=null)v9=this.Trim(v9.toString());
return v9;
}
this.SetValue=function (e4,h1,h3,x4,noEvent,recalc){
h1=this.GetDisplayIndex(e4,h1);
if (x4!=null&&typeof(x4)!="string")x4=new String(x4);
var h4=this.GetCellFromRowCol(e4,h1,h3);
if (this.ValidateCell(e4,h4,x4)){
this.SetCellValueFromView(h4,x4);
if (x4!=null){
this.SetCellValue(e4,h4,""+x4,noEvent,recalc);
}else {
this.SetCellValue(e4,h4,"",noEvent,recalc);
}
this.SizeSpread(e4);
}else {
if (e4.getAttribute("lcidMsg")!=null)
alert(e4.getAttribute("lcidMsg"));
else 
alert("Can't set the data into the cell. The data type is not correct for the cell.");
}
}
this.SetActiveCell=function (e4,h1,h3){
this.ClearSelection(e4,true);
h1=this.GetDisplayIndex(e4,h1);
this.UpdateAnchorCell(e4,h1,h3);
this.ResetLeadingCell(e4);
}
this.GetOperationMode=function (e4){
var o2=e4.getAttribute("OperationMode");
return o2;
}
this.SetOperationMode=function (e4,o2){
e4.setAttribute("OperationMode",o2);
}
this.GetEnableRowEditTemplate=function (e4){
var ab9=e4.getAttribute("EnableRowEditTemplate");
return ab9;
}
this.GetSelectionPolicy=function (e4){
var ac0=e4.getAttribute("SelectionPolicy");
return ac0;
}
this.UpdateAnchorCell=function (e4,h1,h3,select,isColHeader){
if (h1<0||h3<0)return ;
if (e4.getAttribute("LayoutMode")&&e4.allowGroup&&isColHeader)
h1=this.GetRowFromViewPort(e4,h1,h3);
e4.d1=this.GetCellFromRowCol(e4,h1,h3);
if (e4.d1==null)return ;
this.SetActiveRow(e4,this.GetRowKeyFromCell(e4,e4.d1));
this.SetActiveCol(e4,e4.getAttribute("LayoutMode")?this.GetColKeyFromCell2(e4,e4.d1):this.GetColKeyFromCell(e4,e4.d1));
if (select==null||select){
var o2=this.GetOperationMode(e4);
if (o2=="RowMode"||o2=="SingleSelect"||o2=="ExtendedSelect")
this.SelectRow(e4,h1,1,true,true);
else if (o2!="MultiSelect")
this.SelectRange(e4,h1,h3,1,1,true);
else 
this.PaintFocusRect(e4);
}
}
this.ResetLeadingCell=function (e4){
if (e4.d1==null||!this.IsChild(e4.d1,e4))return ;
e4.d3=this.GetRowFromCell(e4,e4.d1);
e4.d4=this.GetColFromCell(e4,e4.d1);
this.SelectRange(e4.d3,e4.d4,1,1,true);
}
this.Edit=function (e4,i9){
var o2=this.GetOperationMode(e4);
if (o2!="RowMode")return ;
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1){
if (FarPoint&&FarPoint.System.WebControl.MultiColumnComboBoxCellTypeUtilitis)
FarPoint.System.WebControl.MultiColumnComboBoxCellTypeUtilitis.CloseAll();
this.SyncData(v6,"Edit,"+i9,e4);
}
else 
__doPostBack(v6,"Edit,"+i9);
}
this.Update=function (e4){
if (this.a7&&this.GetOperationMode(e4)!="RowMode"&&this.GetEnableRowEditTemplate(e4)!="true")return ;
this.SaveData(e4);
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Update",e4);
else 
__doPostBack(v6,"Update");
}
this.Cancel=function (e4){
var g0=document.getElementById(e4.id+"_data");
g0.value="";
this.SaveData(e4);
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Cancel",e4);
else 
__doPostBack(v6,"Cancel");
}
this.Add=function (e4){
if (this.a7)return ;
var v6=null;
var p9=this.GetPageActiveSpread();
if (p9!=null){
v6=p9.getAttribute("name");
}else {
v6=e4.getAttribute("name");
}
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Add",e4);
else 
__doPostBack(v6,"Add");
}
this.Insert=function (e4){
if (this.a7)return ;
var v6=null;
var p9=this.GetPageActiveSpread();
if (p9!=null){
v6=p9.getAttribute("name");
}else {
v6=e4.getAttribute("name");
}
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Insert",e4);
else 
__doPostBack(v6,"Insert");
}
this.Delete=function (e4){
if (this.a7)return ;
var v6=null;
var p9=this.GetPageActiveSpread();
if (p9!=null){
v6=p9.getAttribute("name");
}else {
v6=e4.getAttribute("name");
}
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Delete",e4);
else 
__doPostBack(v6,"Delete");
}
this.Print=function (e4){
if (this.a7)return ;
this.SaveData(e4);
if (document.printSpread==null){
var g0=document.createElement("IFRAME");
g0.name="printSpread";
g0.style.position="absolute";
g0.style.left="-10px";
g0.style.width="0px";
g0.style.height="0px";
document.printSpread=g0;
document.body.insertBefore(g0,null);
g0.addEventListener("load",function (){the_fpSpread.PrintSpread();},false);
}
var ac2=this.GetForm(e4);
if (ac2==null)return ;
{
var i3=ac2;
i3.__EVENTTARGET.value=e4.getAttribute("name");
i3.__EVENTARGUMENT.value="Print";
var ac3=i3.target;
i3.target="printSpread";
i3.submit();
i3.target=ac3;
}
}
this.PrintSpread=function (){
document.printSpread.contentWindow.focus();
document.printSpread.contentWindow.print();
window.focus();
var p9=this.GetPageActiveSpread();
if (p9!=null)this.Focus(p9);
}
this.GotoPage=function (e4,f0){
if (this.a7)return ;
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Page,"+f0,e4);
else 
__doPostBack(v6,"Page,"+f0);
}
this.Next=function (e4){
if (this.a7)return ;
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Next",e4);
else 
__doPostBack(v6,"Next");
}
this.Prev=function (e4){
if (this.a7)return ;
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Prev",e4);
else 
__doPostBack(v6,"Prev");
}
this.GetViewportFromCell=function (e4,j2){
if (j2!=null){
var g0=j2;
while (g0!=null){
if (g0.tagName=="TABLE")break ;
g0=g0.parentNode;
}
if (g0==this.GetViewport(e4)||g0==this.GetViewport0(e4)||g0==this.GetViewport1(e4)||g0==this.GetViewport2(e4))
return g0;
}
return null;
}
this.IsChild=function (h4,i8){
if (h4==null||i8==null)return false;
var g0=h4.parentNode;
while (g0!=null){
if (g0==i8)return true;
g0=g0.parentNode;
}
return false;
}
this.GetCorner=function (e4){
return e4.c4;
}
this.GetFooterCorner=function (e4){
return e4.footerCorner;
}
this.Select=function (e4,cl1,cl2){
if (this.GetSpread(cl1)!=e4||this.GetSpread(cl2)!=e4)return ;
var h5=e4.d5;
var h6=e4.d6;
var ac4=this.GetRowFromCell(e4,cl2);
var n0=0;
if (e4.d7=="r"){
n0=-1;
if (this.IsChild(cl2,this.GetColHeader(e4)))
ac4=0;
}else if (e4.d7=="c"){
if (this.IsChild(cl2,this.GetRowHeader(e4)))
n0=0;
else 
n0=this.GetColFromCell(e4,cl2);
ac4=-1;
}
else {
if (this.IsChild(cl2,this.GetColHeader(e4))){
ac4=0;n0=this.GetColFromCell(e4,cl2);
}else if (this.IsChild(cl2,this.GetRowHeader(e4))){
n0=0;
}else {
n0=this.GetColFromCell(e4,cl2);
}
}
if (e4.d7=="t"){
h6=n0=h5=ac4=-1;
}
var g0=Math.max(h5,ac4);
h5=Math.min(h5,ac4);
ac4=g0;
g0=Math.max(h6,n0);
h6=Math.min(h6,n0);
n0=g0;
var h9=null;
var o0=this.GetSelection(e4);
var o1=o0.lastChild;
if (o1!=null){
var h1=this.GetRowByKey(e4,o1.getAttribute("row"));
var h3=this.GetColByKey(e4,o1.getAttribute("col"));
var n6=parseInt(o1.getAttribute("rowcount"));
var h0=parseInt(o1.getAttribute("colcount"));
h9=new this.Range();
this.SetRange(h9,"cell",h1,h3,n6,h0);
}
if (h9!=null&&h9.col==-1&&h9.row==-1)return ;
if (h9!=null&&h9.col==-1&&h9.row>=0){
if (h9.row>ac4||h9.row+h9.rowCount-1<h5){
this.PaintSelection(e4,h9.row,h9.col,h9.rowCount,h9.colCount,false);
this.PaintSelection(e4,h5,h6,ac4-h5+1,n0-h6+1,true);
}else {
if (h5>h9.row){
var g0=h5-h9.row;
this.PaintSelection(e4,h9.row,h9.col,g0,h9.colCount,false);
if (ac4<h9.row+h9.rowCount-1){
this.PaintSelection(e4,ac4,h9.col,h9.row+h9.rowCount-ac4,h9.colCount,false);
}else {
this.PaintSelection(e4,h9.row+h9.rowCount,h9.col,ac4-h9.row-h9.rowCount+1,h9.colCount,true);
}
}else {
this.PaintSelection(e4,h5,h9.col,h9.row-h5,h9.colCount,true);
if (ac4<h9.row+h9.rowCount-1){
this.PaintSelection(e4,ac4+1,h9.col,h9.row+h9.rowCount-ac4-1,h9.colCount,false);
}else {
this.PaintSelection(e4,h9.row+h9.rowCount,h9.col,ac4-h9.row-h9.rowCount+1,h9.colCount,true);
}
}
}
}else if (h9!=null&&h9.row==-1&&h9.col>=0){
if (h9.col>n0||h9.col+h9.colCount-1<h6){
this.PaintSelection(e4,h9.row,h9.col,h9.rowCount,h9.colCount,false);
this.PaintSelection(e4,h5,h6,ac4-h5+1,n0-h6+1,true);
}else {
if (h6>h9.col){
this.PaintSelection(e4,h9.row,h9.col,h9.rowCount,h6-h9.col,false);
if (n0<h9.col+h9.colCount-1){
this.PaintSelection(e4,h9.row,n0,h9.rowCount,h9.col+h9.colCount-n0,false);
}else {
this.PaintSelection(e4,h9.row,h9.col+h9.colCount,h9.rowCount,n0-h9.col-h9.colCount,true);
}
}else {
this.PaintSelection(e4,h9.row,h6,h9.rowCount,h9.col-h6,true);
if (n0<h9.col+h9.colCount-1){
this.PaintSelection(e4,h9.row,n0+1,h9.rowCount,h9.col+h9.colCount-n0-1,false);
}else {
this.PaintSelection(e4,h9.row,h9.col+h9.colCount,h9.rowCount,n0-h9.col-h9.colCount+1,true);
}
}
}
}else if (h9!=null&&h9.row>=0&&h9.col>=0){
this.ExtendSelection(e4,h9,h5,h6,ac4-h5+1,n0-h6+1);
}else {
this.PaintSelection(e4,h5,h6,ac4-h5+1,n0-h6+1,true);
}
this.SetSelection(e4,h5,h6,ac4-h5+1,n0-h6+1,h9==null);
}
this.ExtendSelection=function (e4,h9,newRow,newCol,newRowCount,newColCount)
{
var r6=Math.max(h9.col,newCol);
var r7=Math.min(h9.col+h9.colCount-1,newCol+newColCount-1);
var x2=Math.max(h9.row,newRow);
var ac5=Math.min(h9.row+h9.rowCount-1,newRow+newRowCount-1);
if (h9.row<x2){
this.PaintSelection(e4,h9.row,h9.col,x2-h9.row,h9.colCount,false);
}
if (h9.col<r6){
this.PaintSelection(e4,h9.row,h9.col,h9.rowCount,r6-h9.col,false);
}
if (h9.row+h9.rowCount-1>ac5){
this.PaintSelection(e4,ac5+1,h9.col,h9.row+h9.rowCount-ac5-1,h9.colCount,false);
}
if (h9.col+h9.colCount-1>r7){
this.PaintSelection(e4,h9.row,r7+1,h9.rowCount,h9.col+h9.colCount-r7-1,false);
}
if (newRow<x2){
this.PaintSelection(e4,newRow,newCol,x2-newRow,newColCount,true);
}
if (newCol<r6){
this.PaintSelection(e4,newRow,newCol,newRowCount,r6-newCol,true);
}
if (newRow+newRowCount-1>ac5){
this.PaintSelection(e4,ac5+1,newCol,newRow+newRowCount-ac5-1,newColCount,true);
}
if (newCol+newColCount-1>r7){
this.PaintSelection(e4,newRow,r7+1,newRowCount,newCol+newColCount-r7-1,true);
}
}
this.PaintAnchorCellHeader=function (e4,select){
var h1,h3;
h1=this.GetRowFromCell(e4,e4.d1);
h3=this.GetColFromCell(e4,e4.d1);
if (select&&e4.d1.getAttribute("group")!=null){
var r8=this.GetSpanCell(h1,h3,e4.d9);
if (r8!=null&&r8.colCount>1){
var ac6=this.GetSelectedRange(e4);
if (h1<ac6.row||h1>=ac6.row+ac6.rowCount||h3<ac6.col||h3>=ac6.col+ac6.colCount)
return ;
}
}
if (this.GetColHeader(e4)!=null)this.PaintHeaderSelection(e4,h1,h3,1,1,select,true);
if (this.GetRowHeader(e4)!=null)this.PaintHeaderSelection(e4,h1,h3,1,1,select,false);
}
this.LineIntersection=function (s1,h6,s2,n0){
var t2,g3;
t2=Math.max(s1,s2);
g3=Math.min(s1+h6,s2+n0);
if (t2<g3)
return {s:t2,c:g3-t2};
return null;
}
this.RangeIntersection=function (h5,h6,y5,cc1,ac4,n0,rc2,cc2){
var ac7=this.LineIntersection(h5,y5,ac4,rc2);
var ac8=this.LineIntersection(h6,cc1,n0,cc2);
if (ac7&&ac8)
return {row:ac7.s,col:ac8.s,rowCount:ac7.c,colCount:ac8.c};
return null;
}
this.PaintSelection=function (e4,h1,h3,n6,h0,select){
if (h1<0&&h3<0){
this.PaintCornerSelection(e4,select);
}
var ac9=false;
var ad0=false;
var u0;
var u6;
var ad1;
if (h1<0){
u0=h1;
h1=0;
n6=this.GetRowCountInternal(e4);
}
if (h3<0){
u6=h3;
h3=0;
h0=this.GetColCount(e4);
}
this.PaintViewportSelection(e4,h1,h3,n6,h0,select);
var o0=this.GetSelection(e4);
var o1;
var ac4;
var n0;
var ad2;
var ad3;
var ad4=0;
var ad5=0;
var h9;
var ad6;
for (var f1=o0.childNodes.length-1;f1>=0;f1--){
o1=o0.childNodes[f1];
if (o1){
ac4=parseInt(o1.getAttribute("rowIndex"));
n0=parseInt(o1.getAttribute("colIndex"));
ad2=parseInt(o1.getAttribute("rowcount"));
ad3=parseInt(o1.getAttribute("colcount"));
if (ac4<0||ad2<0){ac4=0;ad2=this.GetRowCountInternal(e4);}
if (n0<0||ad3<0){n0=0;ad3=this.GetColCount(e4);}
if (ad4<ad2)
ad4=ad2;
if (ad5<ad3)
ad5=ad3;
if (f1>=o0.childNodes.length-1){
if (h1<=ac4&&n6>=ad2||e4.getAttribute("LayoutMode")&&h0==1&&h1<parseInt(e4.getAttribute("layoutrowcount"))){
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal"){
this.PaintHeaderSelection(e4,h1,h3,n6,h0,select,true);
ac9=true;
}
}
if (h3<=n0&&h0>=ad3){
if (this.GetRowHeader(e4)!=null){
this.PaintHeaderSelection(e4,h1,h3,n6,h0,select,false);
ad0=true;
}
}
if (!ac9&&!ad0){
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal"){
this.PaintHeaderSelection(e4,h1,h3,n6,h0,select,true);
ac9=true;
}
if (this.GetRowHeader(e4)!=null){
this.PaintHeaderSelection(e4,h1,h3,n6,h0,select,false);
ad0=true;
}
}
}
else {
if (!select&&this.GetOperationMode(e4)=="Normal"&&!e4.getAttribute("LayoutMode")){
h9=this.RangeIntersection(h1,h3,n6,h0,ac4,n0,ad2,ad3);
if (h9){
this.PaintViewportSelection(e4,h9.row,h9.col,h9.rowCount,h9.colCount,true);
}
if (ac9){
ad6=this.LineIntersection(h3,h0,n0,ad3);
if (ad6)this.PaintHeaderSelection(e4,h1,ad6.s,n6,ad6.c,true,true);
}
if (ad0){
ad6=this.LineIntersection(h1,n6,ac4,ad2);
if (ad6)this.PaintHeaderSelection(e4,ad6.s,h3,ad6.c,h0,true,false);
}
}
}
}
}
if (u0!=null||u6!=null){
if ((ad4<n6&&u0<0)||(ad5<h0&&u6<0))
ad1=true;
}
if (o0.childNodes.length<=0||(e4.getAttribute("SelectionPolicy")=="MultiRange"&&ad1)){
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal")this.PaintHeaderSelection(e4,h1,h3,n6,h0,select,true);
if (this.GetRowHeader(e4)!=null)this.PaintHeaderSelection(e4,h1,h3,n6,h0,select,false);
}
this.PaintAnchorCell(e4);
}
this.PaintFocusRect=function (e4){
var g7=document.getElementById(e4.id+"_focusRectT");
if (g7==null)return ;
var ad7=this.GetSelectedRange(e4);
if (e4.d1==null&&(ad7==null||(ad7.rowCount==0&&ad7.colCount==0))){
g7.style.left="-1000px";
var v6=e4.id;
g7=document.getElementById(v6+"_focusRectB");
g7.style.left="-1000px";
g7=document.getElementById(v6+"_focusRectL");
g7.style.left="-1000px";
g7=document.getElementById(v6+"_focusRectR");
g7.style.left="-1000px";
return ;
}
var i2=this.GetOperationMode(e4);
if (i2=="RowMode"||i2=="SingleSelect"||i2=="MultiSelect"||i2=="ExtendedSelect"){
var h1=e4.GetActiveRow();
if (e4.getAttribute("layoutMode"))
h1=this.GetFirstMultiRowFromViewport(e4,h1,false);
ad7=new this.Range();
this.SetRange(ad7,"Row",h1,-1,1,-1);
}else if (ad7==null||(ad7.rowCount==0&&ad7.colCount==0)){
var h1=e4.GetActiveRow();
var h3=e4.GetActiveCol();
ad7=new this.Range();
this.SetRange(ad7,"Cell",h1,h3,e4.d1.rowSpan,e4.d1.colSpan);
}
if (ad7.row<0){
ad7.row=0;
ad7.rowCount=this.GetRowCountInternal(e4);
}
if (ad7.col<0){
ad7.col=0;
ad7.colCount=this.GetColCount(e4);
if (e4.getAttribute("LayoutMode")&&ad7.rowCount<parseInt(e4.getAttribute("layoutrowcount"))&&ad7.type=="Row")ad7.rowCount=parseInt(e4.getAttribute("layoutrowcount"));
}
var u0;
if (e4.getAttribute("LayoutMode"))
u0=(ad7.innerRow!=null)?ad7.innerRow:ad7.row;
else 
u0=ad7.row;
var h4=this.GetCellFromRowCol(e4,u0,ad7.col);
if (h4==null)return ;
if (ad7.rowCount==1&&ad7.colCount==1){
ad7.rowCount=h4.rowSpan;
ad7.colCount=h4.colSpan;
if (h4.colSpan>1){
var ad8=parseInt(h4.getAttribute("col"));
if (ad8!=ad7.col&&!isNaN(ad8)&&!e4.getAttribute("LayoutMode"))ad7.col=ad8;
}
}
var g0=this.GetOffsetTop(e4,h4);
var ad9=this.GetOffsetLeft(e4,h4);
if (h4.rowSpan>1){
u0=h4.parentNode.rowIndex;
var h6=this.GetCellFromRowCol(e4,u0,ad7.col+ad7.colCount-1);
if (h6!=null&&h6.parentNode.rowIndex>h4.parentNode.rowIndex){
g0=this.GetOffsetTop(e4,h6);
}
if (e4.getAttribute("LayoutMode")&&ad7.rowCount<h4.rowSpan&&(ad7.type=="Column"||ad7.type=="Row"))ad7.rowCount=h4.rowSpan;
}
if (h4.colSpan>1){
var h6=this.GetCellFromRowCol(e4,u0+ad7.rowCount-1,ad7.col);
var q1=this.GetOffsetLeft(e4,h6);
if (q1>ad9){
ad9=q1;
h4=h6;
}
if (e4.getAttribute("LayoutMode")&&ad7.colCount<h4.colSpan&&(ad7.type=="Column"||ad7.type=="Row"))ad7.colCount=h4.colSpan;
}
var k0=0;
var g9=this.GetViewport(e4).rows;
for (var h1=u0;h1<u0+ad7.rowCount&&h1<g9.length;h1++){
k0+=g9[h1].offsetHeight;
if (h1>u0)k0+=parseInt(this.GetViewport(e4).cellSpacing);
}
var j3=0;
var f3=this.GetColGroup(this.GetViewport(e4));
if (f3.childNodes==null||f3.childNodes.length==0)return ;
for (var h3=ad7.col;h3<ad7.col+ad7.colCount&&h3<f3.childNodes.length;h3++){
j3+=f3.childNodes[h3].offsetWidth;
if (h3>ad7.col)j3+=parseInt(this.GetViewport(e4).cellSpacing);
}
if (ad7.col>h4.cellIndex&&ad7.type=="Column"){
var n0=(e4.getAttribute("LayoutMode")!=null)?parseInt(h4.getAttribute("scol")):parseInt(h4.getAttribute("col"));
for (var h3=n0;h3<ad7.col;h3++){
ad9+=f3.childNodes[h3].offsetWidth;
if (h3>n0)ad9+=parseInt(this.GetViewport(e4).cellSpacing);
}
}
if (ad7.row>0)g0-=2;
else k0-=2;
if (ad7.col>0)ad9-=2;
else j3-=2;
if (parseInt(this.GetViewport(e4).cellSpacing)>0){
g0+=1;ad9+=1;
}else {
j3+=1;
k0+=1;
}
if (j3<0)j3=0;
if (k0<0)k0=0;
g7.style.left=""+ad9+"px";
g7.style.top=""+g0+"px";
g7.style.width=""+j3+"px";
g7=document.getElementById(e4.id+"_focusRectB");
g7.style.left=""+ad9+"px";
g7.style.top=""+(g0+k0)+"px";
g7.style.width=""+j3+"px";
g7=document.getElementById(e4.id+"_focusRectL");
g7.style.left=""+ad9+"px";
g7.style.top=""+g0+"px";
g7.style.height=""+k0+"px";
g7=document.getElementById(e4.id+"_focusRectR");
g7.style.left=""+(ad9+j3)+"px";
g7.style.top=""+g0+"px";
g7.style.height=""+k0+"px";
}
this.PaintCornerSelection=function (e4,select){
var ae0=true;
if (this.GetTopSpread(e4).getAttribute("ShowHeaderSelection")=="false")ae0=false;
if (!ae0)return ;
var n7=this.GetCorner(e4);
if (n7!=null&&n7.rows.length>0){
for (var f1=0;f1<n7.rows.length;f1++){
for (var i1=0;i1<n7.rows[f1].cells.length;i1++){
if (n7.rows[f1].cells[i1]!=null)
this.PaintSelectedCell(e4,n7.rows[f1].cells[i1],select);
}
}
}
}
this.PaintHeaderSelection=function (e4,h1,h3,n6,h0,select,c6){
var ae0=true;
if (this.GetTopSpread(e4).getAttribute("ShowHeaderSelection")=="false")ae0=false;
if (!ae0)return ;
var ae1=c6?e4.e1:e4.e0;
if (e4.getAttribute("LayoutMode")&&c6){
if (n6>parseInt(e4.getAttribute("layoutrowcount")))
n6=parseInt(e4.getAttribute("layoutrowcount"));
var ae2=this.GetCellFromRowCol(e4,h1,h3);
if (e4.allowGroup&&ae2.getAttribute("group")!=null)
h1=this.GetFirstMultiRowFromViewport(e4,h1,true);
for (var f1=h1;f1<h1+n6;f1++){
for (var i1=h3;i1<h3+h0;i1++){
var ae3=this.GetCellFromRowCol(e4,f1,i1);
if (ae3){
var o3=this.GetRowTemplateRowFromCell(e4,ae3);
if (!isNaN(o3)){
if (o3>=parseInt(e4.getAttribute("layoutrowcount")))o3=parseInt(e4.getAttribute("layoutrowcount"))-1;
var h4=this.GetHeaderCellFromRowCol(e4,o3,i1,c6);
if (h4!=null&&this.GetRowTemplateRowFromCell(e4,h4)==o3)this.PaintSelectedCell(e4,h4,select);
}
}
}
}
}
else {
var u1=this.GetRowCountInternal(e4);
var t6=this.GetColCount(e4);
if (c6){
if (this.GetColHeader(e4)==null)return ;
h1=0;
n6=u1=this.GetColHeader(e4).rows.length;
}else {
if (this.GetRowHeader(e4)==null)return ;
h3=0;
h0=t6=this.GetColGroup(this.GetRowHeader(e4)).childNodes.length;
}
}
if (e4.getAttribute("LayoutMode")&&e4.getAttribute("OperationMode")!="Normal"&&!c6)
h1=this.GetFirstMultiRowFromViewport(e4,h1,false);
if (e4.getAttribute("LayoutMode")&&e4.d1!=null&&e4.d1.getAttribute("group")!=null&&!c6&&n6!=u1)
n6=1;
for (var f1=h1;f1<h1+n6&&f1<u1;f1++){
if (!c6&&this.IsChildSpreadRow(e4,this.GetViewport(e4),f1))continue ;
for (var i1=h3;i1<h3+h0&&i1<t6;i1++){
if (!e4.getAttribute("LayoutMode")&&this.IsCovered(e4,f1,i1,ae1))continue ;
var h4=this.GetHeaderCellFromRowCol(e4,f1,i1,c6);
if (h4!=null)this.PaintSelectedCell(e4,h4,select);
}
}
}
this.PaintViewportSelection=function (e4,h1,h3,n6,h0,select){
var u1=this.GetRowCountInternal(e4);
var t6=this.GetColCount(e4);
if (e4.getAttribute("LayoutMode")&&e4.getAttribute("OperationMode")!="Normal"&&n6==parseInt(e4.getAttribute("layoutrowcount")))
h1=this.GetFirstMultiRowFromViewport(e4,h1,false);
if (e4.getAttribute("LayoutMode")&&e4.d1!=null&&e4.d1.getAttribute("group")!=null&&n6!=u1)
n6=1;
for (var f1=h1;f1<h1+n6&&f1<u1;f1++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),f1))continue ;
var h4=null;
for (var i1=h3;i1<h3+h0&&i1<t6;i1++){
if (this.IsCovered(e4,f1,i1,e4.d9))continue ;
h4=this.GetCellFromRowCol(e4,f1,i1,h4);
this.PaintSelectedCell(e4,h4,select);
}
}
}
this.Copy=function (e4){
var p9=this.GetPageActiveSpread();
if (p9!=null&&p9!=e4&&this.GetTopSpread(p9)==e4){
this.Copy(p9);
return ;
}
var o0=this.GetSelection(e4);
var o1=o0.lastChild;
if (o1!=null){
var h1;
var h3;
var n6;
var h0;
e4.copymulticol=false;
if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")!="-1"&&o1.getAttribute("row")=="-1"&&o1.getAttribute("rowcount")=="-1"){
var h4=e4.d1;
if (h4){
h1=h4.parentNode.getAttribute("row");
h3=this.GetColFromCell(e4,h4);
n6=this.GetRowCountInternal(e4);
h0=parseInt(o1.getAttribute("colcount"));
e4.copymulticol=true;
}
}
else if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")=="-1"&&o1.getAttribute("row")!=-1){
var u3=parseInt(o1.getAttribute("row"));
h1=this.GetFirstRowFromKey(e4,u3);
h3=parseInt(o1.getAttribute("colIndex"));
n6=parseInt(e4.getAttribute("layoutrowcount"));
}
else {
h1=e4.getAttribute("LayoutMode")?parseInt(o1.getAttribute("rowIndex")):this.GetRowByKey(e4,o1.getAttribute("row"));
h3=e4.getAttribute("LayoutMode")?parseInt(o1.getAttribute("colIndex")):this.GetColByKey(e4,o1.getAttribute("col"));
n6=parseInt(o1.getAttribute("rowcount"));
h0=parseInt(o1.getAttribute("colcount"));
}
if (h1<0){
h1=0;
n6=this.GetRowCountInternal(e4);
}
if (h3<0){
h3=0;
h0=this.GetColCount(e4);
}
var f6="";
for (var f1=h1;f1<h1+n6;f1++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),f1))continue ;
var h4=null;
for (var i1=h3;i1<h3+h0;i1++){
if (this.IsCovered(e4,f1,i1,e4.d9))
f6+="";
else 
{
h4=this.GetCellFromRowCol(e4,f1,i1,h4);
if (e4.getAttribute("LayoutMode")&&e4.copymulticol&&(h4==null||(h4.parentNode.getAttribute("row"))!=h1))continue ;
if (h4!=null&&h4.parentNode.getAttribute("previewrow")!=null)continue ;
var j5=this.GetCellType(h4);
if (j5=="TextCellType"&&h4.getAttribute("password")!=null)
f6+="";
else 
f6+=this.GetCellValueFromView(e4,h4);
}
if (i1+1<h3+h0)f6+="\t";
}
f6+="\r\n";
}
this.b9=f6;
}else {
if (e4.d1!=null){
var f6=this.GetCellValueFromView(e4,e4.d1);
this.b9=f6;
}
}
}
this.GetCellValueFromView=function (e4,h4){
var x4=null;
if (h4!=null){
var ae4=this.GetRender(h4);
x4=this.GetValueFromRender(e4,ae4);
if (x4==null||x4==" ")x4="";
}
return x4;
}
this.SetCellValueFromView=function (h4,x4,ignoreLock){
if (h4!=null){
var ae4=this.GetRender(h4);
var t1=this.GetCellType(h4);
if ((t1!="readonly"||ignoreLock)&&ae4!=null&&ae4.getAttribute("FpEditor")!="Button")
this.SetValueToRender(ae4,x4);
}
}
this.Paste=function (e4){
var p9=this.GetPageActiveSpread();
if (p9!=null&&p9!=e4&&this.GetTopSpread(p9)==e4){
this.Paste(p9);
return ;
}
if (e4.d1==null)return ;
var f6=this.b9;
if (f6==null)return ;
var e7=this.GetViewportFromCell(e4,e4.d1);
var h1=this.GetRowFromCell(e4,e4.d1);
var h3=this.GetColFromCell(e4,e4.d1);
var h0=this.GetColCount(e4);
var n6=this.GetRowCountInternal(e4);
var ae5=h1;
var aa4=h3;
var ae6=new String(f6);
if (ae6.length==0)return ;
var f0=ae6.lastIndexOf("\r\n");
if (f0>=0&&f0==ae6.length-2)ae6=ae6.substring(0,f0);
var ae7=0;
var ae8=ae6.split("\r\n");
for (var f1=0;f1<ae8.length&&ae5<n6;f1++){
if (typeof(ae8[f1])=="string"){
ae8[f1]=ae8[f1].split("\t");
if (ae8[f1].length>ae7)ae7=ae8[f1].length;
}
ae5++;
}
ae5=this.GetSheetIndex(e4,h1);
for (var f1=0;f1<ae8.length&&ae5<n6;f1++){
var ae9=ae8[f1];
if (ae9!=null){
aa4=h3;
var h4=null;
var ac4=this.GetDisplayIndex(e4,ae5);
for (var i1=0;i1<ae9.length&&aa4<h0;i1++){
if (!this.IsCovered(e4,ac4,aa4,e4.d9)){
h4=this.GetCellFromRowCol(e4,ac4,aa4,h4);
if (h4==null)return ;
if (e4.getAttribute("LayoutMode")&&e4.copymulticol&&parseInt(h4.parentNode.getAttribute("row"))!=parseInt(e4.d1.parentNode.getAttribute("row")))continue ;
if (h4!=null&&h4.parentNode.getAttribute("previewrow")!=null)continue ;
var af0=ae9[i1];
if (!this.ValidateCell(e4,h4,af0)){
if (e4.getAttribute("lcidMsg")!=null)
alert(e4.getAttribute("lcidMsg"));
else 
alert("Can't set the data into the cell. The data type is not correct for the cell.");
return ;
}
}
aa4++;
}
}
ae5++;
}
if (ae8.length==0)return ;
ae5=this.GetSheetIndex(e4,h1);
for (var f1=0;f1<ae8.length&&ae5<n6;f1++){
aa4=h3;
var ae9=ae8[f1];
var h4=null;
var ac4=this.GetDisplayIndex(e4,ae5);
for (var i1=0;i1<ae7&&aa4<h0;i1++){
if (!this.IsCovered(e4,ac4,aa4,e4.d9)){
h4=this.GetCellFromRowCol(e4,ac4,aa4,h4);
if (e4.getAttribute("LayoutMode")&&e4.copymulticol&&parseInt(h4.parentNode.getAttribute("row"))!=parseInt(e4.d1.parentNode.getAttribute("row")))continue ;
if (h4!=null&&h4.parentNode.getAttribute("previewrow")!=null)continue ;
var t1=this.GetCellType(h4);
var ae4=this.GetRender(h4);
if (t1!="readonly"&&ae4.getAttribute("FpEditor")!="Button"){
var af0=null;
if (ae9!=null&&i1<ae9.length)af0=ae9[i1];
this.SetCellValueFromView(h4,af0);
if (af0!=null){
this.SetCellValue(e4,h4,""+af0);
}else {
this.SetCellValue(e4,h4,"");
}
}
}
aa4++;
}
ae5++;
}
var x9=e4.getAttribute("autoCalc");
if (x9!="false"){
this.UpdateValues(e4);
}
var e9=this.GetTopSpread(e4);
var g1=document.getElementById(e9.id+"_textBox");
if (g1!=null){
g1.blur();
}
this.Focus(e4);
this.SizeSpread(e4);
}
this.UpdateValues=function (e4){
if (e4.d8==null&&this.GetParentSpread(e4)==null&&e4.getAttribute("rowFilter")!="true"&&e4.getAttribute("hierView")!="true"&&e4.getAttribute("IsNewRow")!="true"){
this.SaveData(e4);
this.StorePostData(e4);
this.SyncData(e4.getAttribute("name"),"UpdateValues",e4);
}
}
this.ValidateCell=function (e4,h4,x4){
if (h4==null||x4==null||x4=="")return true;
var x7=null;
var j5=this.GetCellType(h4);
if (j5!=null){
var i3=this.GetFunction(j5+"_isValid");
if (i3!=null){
x7=i3(h4,x4);
}
}
return (x7==null||x7=="");
}
this.DoclearSelection=function (e4){
var o0=this.GetSelection(e4);
var o1=o0.lastChild;
while (o1!=null){
var h1=e4.getAttribute("LayoutMode")?parseInt(o1.getAttribute("rowIndex")):this.GetRowByKey(e4,o1.getAttribute("row"));
var h3=e4.getAttribute("LayoutMode")?parseInt(o1.getAttribute("colIndex")):this.GetColByKey(e4,o1.getAttribute("col"));
var n6=parseInt(o1.getAttribute("rowcount"));
var h0=parseInt(o1.getAttribute("colcount"));
if (e4.getAttribute("LayoutMode")&&h1!=-1&&(h0==-1||e4.getAttribute("OperationMode")!="Normal")){
n6=parseInt(e4.getAttribute("layoutrowcount"));
this.PaintSelection(e4,h1,-1,n6,-1,false);
}
if (e4.getAttribute("LayoutMode")&&h3!=-1&&(n6==-1||e4.getAttribute("OperationMode")!="Normal")){
var i9=this.GetRowTemplateRowFromGroupCell(e4,parseInt(o1.getAttribute("col")));
var h4=this.GetCellByRowCol2(e4,i9,parseInt(o1.getAttribute("col")));
if (h4){
h1=parseInt(h4.parentNode.getAttribute("row"));
h3=this.GetColFromCell(e4,h4);
}
this.PaintMultipleRowSelection(e4,h1,h3,1,h0,false);
}
else 
this.PaintSelection(e4,h1,h3,n6,h0,false);
o0.removeChild(o1);
o1=o0.lastChild;
}
}
this.Clear=function (e4){
var p9=this.GetPageActiveSpread();
if (p9!=null&&p9!=e4&&this.GetTopSpread(p9)==e4){
this.Clear(p9);
return ;
}
var t1=this.GetCellType(e4.d1);
if (t1=="readonly")return ;
var o0=this.GetSelection(e4);
var o1=o0.lastChild;
if (this.AnyReadOnlyCell(e4,o1)){
return ;
}
this.Copy(e4);
if (o1!=null){
var h1;
var h3;
var n6;
var h0;
var af1=false;
if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")!="-1"&&o1.getAttribute("row")=="-1"&&o1.getAttribute("rowcount")=="-1"){
var h4=e4.d1;
if (h4){
h1=h4.parentNode.getAttribute("row");
h3=this.GetColFromCell(e4,h4);
n6=this.GetRowCountInternal(e4);
h0=parseInt(o1.getAttribute("colcount"));
af1=true;
}
}
else if (e4.getAttribute("LayoutMode")&&o1.getAttribute("col")=="-1"&&o1.getAttribute("row")!=-1){
var u3=parseInt(o1.getAttribute("row"));
h1=this.GetFirstRowFromKey(e4,u3);
h3=parseInt(o1.getAttribute("colIndex"));
n6=parseInt(e4.getAttribute("layoutrowcount"));
}
else {
h1=e4.getAttribute("LayoutMode")?parseInt(o1.getAttribute("rowIndex")):this.GetRowByKey(e4,o1.getAttribute("row"));
h3=e4.getAttribute("LayoutMode")?parseInt(o1.getAttribute("colIndex")):this.GetColByKey(e4,o1.getAttribute("col"));
n6=parseInt(o1.getAttribute("rowcount"));;
h0=parseInt(o1.getAttribute("colcount"));
}
if (h1<0){
h1=0;
n6=this.GetRowCountInternal(e4);
}
if (h3<0){
h3=0;
h0=this.GetColCount(e4);
}
for (var f1=h1;f1<h1+n6;f1++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),f1))continue ;
var h4=null;
for (var i1=h3;i1<h3+h0;i1++){
if (!this.IsCovered(e4,f1,i1,e4.d9)){
h4=this.GetCellFromRowCol(e4,f1,i1,h4);
if (e4.getAttribute("LayoutMode")&&af1&&(h4==null||(h4.parentNode.getAttribute("row"))!=h1))continue ;
if (h4!=null&&h4.parentNode.getAttribute("previewrow")!=null)continue ;
var t1=this.GetCellType(h4);
if (t1!="readonly"){
var af2=this.GetEditor(h4);
if (af2!=null&&af2.getAttribute("FpEditor")=="Button")continue ;
this.SetCellValueFromView(h4,null);
this.SetCellValue(e4,h4,"");
}
}
}
}
var x9=e4.getAttribute("autoCalc");
if (x9!="false"){
this.UpdateValues(e4);
}
}
}
this.AnyReadOnlyCell=function (e4,o1){
if (o1!=null){
var h1=this.GetRowByKey(e4,o1.getAttribute("row"));
var h3=this.GetColByKey(e4,o1.getAttribute("col"));
var n6=parseInt(o1.getAttribute("rowcount"));
var h0=parseInt(o1.getAttribute("colcount"));
if (h1<0){
h1=0;
n6=this.GetRowCountInternal(e4);
}
if (h3<0){
h3=0;
h0=this.GetColCount(e4);
}
for (var f1=h1;f1<h1+n6;f1++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),f1))continue ;
var h4=null;
for (var i1=h3;i1<h3+h0;i1++){
if (!this.IsCovered(e4,f1,i1,e4.d9)){
h4=this.GetCellFromRowCol(e4,f1,i1,h4);
var t1=this.GetCellType(h4);
if (t1=="readonly"){
return true;
}
}
}
}
}
return false;
}
this.GetViewportFromPoint=function (e4,m6,n4){
var y1=l2=0;
var q1=t2=0;
var k6=u9=0;
var r3=h2=0;
var m1=this.GetViewport0(e4);
var m2=this.GetViewport1(e4);
var m3=this.GetViewport2(e4);
var e7=this.GetViewport(e4);
if (m1!=null){
y1=this.GetOffsetLeft(e4,m1,document.body);
k6=m1.offsetWidth;
q1=this.GetOffsetTop(e4,m1,document.body);
r3=m1.offsetHeight;
}
if (m3!=null){
y1=this.GetOffsetLeft(e4,m3,document.body);
k6=m3.offsetWidth;
t2=this.GetOffsetTop(e4,m3,document.body);
h2=m3.offsetHeight;
}
if (m2!=null){
l2=this.GetOffsetLeft(e4,m2,document.body);
u9=m2.offsetWidth;
q1=this.GetOffsetTop(e4,m2,document.body);
r3=m2.offsetHeight;
}
if (e7!=null){
l2=this.GetOffsetLeft(e4,e7,document.body);
u9=e7.offsetWidth;
t2=this.GetOffsetTop(e4,e7,document.body);
h2=e7.offsetHeight;
}
if (y1<m6&&m6<l2){
if (q1<n4&&n4<t2)return m1;
else if (t2<n4&&n4<t2+u9)return m3;
}else if (l2<m6&&m6<l2+u9){
if (q1<n4&&n4<t2)return m2;
else if (t2<n4&&n4<t2+u9)return e7;
}
return null;
}
this.GetCellFromPoint=function (e4,m6,n4,e7){
var r6=this.GetOffsetLeft(e4,e7,document.body);
var x2=this.GetOffsetTop(e4,e7,document.body);
if (m6<r6||n4<x2){
return null;
}else {
var g9=e7.rows;
var af3=null;
for (var h1=0;h1<g9.length;h1++){
var m9=g9[h1];
x2+=m9.offsetHeight;
if (n4<x2){
af3=m9;
break ;
}
}
if (af3!=null){
for (var h3=0;h3<af3.cells.length;h3++){
var af4=af3.cells[h3];
r6+=af4.offsetWidth;
if (m6<r6){
return af4;
}
}
}
}
return null;
}
this.MoveSliderBar=function (e4,g3){
var m8=this.GetElementById(this.activePager,e4.id+"_slideBar");
var g0=(g3.clientX-this.GetOffsetLeft(e4,e4,document.body)+window.scrollX-8);
if (g0<e4.slideLeft)g0=e4.slideLeft;
if (g0>e4.slideRight)g0=e4.slideRight;
var n2=parseInt(this.activePager.getAttribute("totalPage"))-1;
var af5=parseInt(((g0-e4.slideLeft)/(e4.slideRight-e4.slideLeft))*n2)+1;
if (e4.style.position!="absolute"&&e4.style.position!="relative")
g0+=this.GetOffsetLeft(e4,e4,document.body)
m8.style.left=g0+"px";
return af5;
}
this.MouseMove=function (event){
if (window.fpPostOn!=null)return ;
event=this.GetEvent(event);
var p1=this.GetTarget(event);
if (p1!=null&&p1.tagName=="scrollbar")
return ;
if (p1.parentNode!=null&&p1.parentNode.getAttribute("previewrow"))
return ;
var e4=this.GetSpread(p1,true);
if (e4!=null&&this.dragSlideBar)
{
if (this.activePager!=null){
var af5=this.MoveSliderBar(e4,event);
var af6=this.GetElementById(this.activePager,e4.id+"_posIndicator");
af6.innerHTML=this.activePager.getAttribute("pageText")+af5;
}
return ;
}
if (this.a6)e4=this.GetSpread(this.b8);
if (e4==null||(!this.a6&&this.HitCommandBar(p1)))return ;
if (e4.getAttribute("OperationMode")=="ReadOnly")return ;
var j9=this.IsXHTML(e4);
if (this.a6){
if (this.dragCol!=null&&this.dragCol>=0){
var z2=this.GetMovingCol(e4);
if (z2!=null){
if (z2.style.display=="none")z2.style.display="";
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
z2.style.top=""+(event.clientY+window.scrollY)+"px";
z2.style.left=""+(event.clientX+window.scrollX+5)+"px";
}else {
z2.style.top=""+(event.clientY-this.GetOffsetTop(e4,e4,document.body)+window.scrollY)+"px";
z2.style.left=""+(event.clientX-this.GetOffsetLeft(e4,e4,document.body)+window.scrollX+5)+"px";
}
}
var e7=this.GetViewport(e4);
var af7=document.body;
var af8=this.GetGroupBar(e4);
var g0=-1;
var m6=event.clientX;
var x2=0;
var r6=0;
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
x2=this.GetOffsetTop(e4,e4,document.body)-e7.parentNode.scrollTop;
r6=this.GetOffsetLeft(e4,e4,document.body)-e7.parentNode.scrollLeft;
m6+=Math.max(document.body.scrollLeft,document.documentElement.scrollLeft);
}else {
m6-=(this.GetOffsetLeft(e4,e4,document.body)-Math.max(document.body.scrollLeft,document.documentElement.scrollLeft));
}
var af9=false;
var j9=this.IsXHTML(e4);
var ag0=j9?document.body.parentNode.scrollTop:document.body.scrollTop;
var m7=document.getElementById(e4.id+"_titleBar");
if (m7)ag0-=m7.parentNode.parentNode.offsetHeight;
if (this.GetPager1(e4)!=null)ag0-=this.GetPager1(e4).offsetHeight;
if (af8!=null&&event.clientY<this.GetOffsetTop(e4,e4,document.body)-e7.parentNode.scrollTop+af8.offsetHeight-ag0){
if (e4.style.position!="absolute"&&e4.style.position!="relative")
r6=this.GetOffsetLeft(e4,e4,document.body);
x2+=10;
af9=true;
var z5=af8.getElementsByTagName("TABLE")[0];
if (z5!=null){
for (var f1=0;f1<z5.rows[0].cells[0].childNodes.length;f1++){
var j3=z5.rows[0].cells[0].childNodes[f1].offsetWidth;
if (j3==null)continue ;
if (r6<=m6&&m6<r6+j3){
g0=f1;
break ;
}
r6+=j3;
}
}
if (g0==-1&&m6>=r6)g0=-2;
e4.targetCol=g0;
}else {
if (e4.style.position=="absolute"||e4.style.position=="relative")
r6=-e7.parentNode.scrollLeft;
if (this.GetRowHeader(e4)!=null)r6+=this.GetRowHeader(e4).offsetWidth;
if (af8!=null)x2+=af8.offsetHeight;
if (m6<r6){
g0=0;
}else {
var aa1=e4.selectedCols.context;
if (aa1){
for (var f1=0;f1<aa1.length;f1++){
if (aa1[f1].left+r6<=m6&&m6<aa1[f1].left+r6+aa1[f1].width){
g0=f1;
}
}
if (this.IsColSelected(e4,g0)){
while (this.IsColSelected(e4,g0)&&this.IsColSelected(e4,g0-1))g0--;
}else {
if (this.IsColSelected(e4,g0-1))g0++;
}
if (g0<0)g0=0;
if (g0>=aa1.length)g0=aa1.length-1;
r6+=aa1[g0].left;
}
}
r6-=5;
var ag1=parseInt(this.GetSheetColIndex(e4,g0));
if (ag1<0)ag1=g0;
e4.targetCol=ag1;
}
if (m7)x2+=m7.parentNode.parentNode.offsetHeight;
if (this.GetPager1(e4)!=null)x2+=this.GetPager1(e4).offsetHeight;
var af6=this.GetPosIndicator(e4);
af6.style.left=""+r6+"px";
af6.style.top=""+x2+"px";
if (af8!=null&&af9&&af8.getElementsByTagName("TABLE").length==0){
af6.style.display="none";
}else {
if (af9||e4.allowColMove)
af6.style.display="";
else 
af6.style.display="none";
}
var i6=this.GetParent(this.GetViewport(e4));
if (i6!=null){
var ag2=this.GetOffsetLeft(e4,e4,document.body)+i6.offsetLeft+i6.offsetWidth-20;
var ag3=0;
var m3=this.GetViewport2(e4);
if (m3!=null){
ag3=m3.offsetWidth;
ag2+=ag3;
}
if (event.clientX>ag2){
i6.scrollLeft=i6.scrollLeft+10;
this.ScrollView(e4);
this.UpdatePostbackData(e4);
}else if (event.clientX<this.GetOffsetLeft(e4,e4,document.body)+i6.offsetLeft+ag3+5){
i6.scrollLeft=i6.scrollLeft-10;
this.ScrollView(e4);
this.UpdatePostbackData(e4);
}
}
return ;
}
if (this.b4==null&&this.b5==null){
if (e4.d1!=null){
var i6=this.GetParent(this.GetViewport(e4));
if (i6!=null){
var t4=this.GetOffsetTop(e4,e4,document.body)+i6.offsetTop+i6.offsetHeight-10;
var ag4=0;
var m2=this.GetViewport1(e4);
if (m2!=null){
ag4=this.GetViewport1(e4).offsetHeight;
t4+=ag4;
}
if (event.clientY>t4){
i6.scrollTop=i6.scrollTop+10;
this.ScrollView(e4);
}else if (event.clientY<this.GetOffsetTop(e4,e4,document.body)+i6.offsetTop+ag4+5){
i6.scrollTop=i6.scrollTop-10;
this.ScrollView(e4);
}
var ag2=this.GetOffsetLeft(e4,e4,document.body)+i6.offsetLeft+i6.offsetWidth-20;
var ag3=0;
var m3=this.GetViewport2(e4);
if (m3!=null){
ag3=m3.offsetWidth;
ag2+=ag3;
}
if (event.clientX>ag2){
i6.scrollLeft=i6.scrollLeft+10;
this.ScrollView(e4);
}else if (event.clientX<this.GetOffsetLeft(e4,e4,document.body)+i6.offsetLeft+ag3+5){
i6.scrollLeft=i6.scrollLeft-10;
this.ScrollView(e4);
}
}
var h4=this.GetCell(p1,null,event);
if (h4==null&&p1!=null){
var e7=this.GetViewportFromPoint(e4,event.clientX,event.clientY);
if (e7!=null)
h4=this.GetCellFromPoint(e4,event.clientX,event.clientY,e7);
}
if (h4!=null&&h4!=e4.d2){
var i2=this.GetOperationMode(e4);
if (i2!="MultiSelect"){
if (i2=="SingleSelect"||i2=="RowMode"){
this.ClearSelection(e4);
var h5=this.GetRowFromCell(e4,h4);
this.UpdateAnchorCell(e4,h5,0);
this.SelectRow(e4,h5,1,true,true);
}else {
if (!(i2=="Normal"&&this.GetSelectionPolicy(e4)=="Single")&&!e4.getAttribute("LayoutMode")){
this.Select(e4,e4.d1,h4);
this.SyncColSelection(e4);
}
}
e4.d2=h4;
}
}
}
}else if (this.b4!=null){
var ag5=event.clientX-this.b6;
var aa2=parseInt(this.b4.width)+ag5;
var x1=0;
var ag6=(aa2>x1);
if (ag6){
if (e4.frzRows>0||e4.frzCols>0){
var h7=0;
if (!j9)h7+=parseInt(e4.style.borderWidth);
e4.sizeBar.style.left=(event.clientX-this.GetOffsetLeft(e4,e4,document.body)-h7+window.scrollX)+"px";
}else {
this.b4.width=aa2;
var k4=parseInt(this.b4.getAttribute("index"));
if (this.IsChild(this.b4,this.GetFrozColHeader(e4)))
this.SetWidthFix(this.GetFrozColHeader(e4),k4,aa2);
else 
this.SetWidthFix(this.GetColHeader(e4),k4-e4.frzCols,aa2);
this.b6=event.clientX;
}
}
}else if (this.b5!=null){
var ag5=event.clientY-this.b7;
var ag7=parseInt(this.b5.style.height)+ag5;
var x1=0;
var ag6=(x1<ag7);
if (ag6){
if (e4.frzRows>0||e4.frzCols>0){
var h7=0;
if (!j9)h7+=parseInt(e4.style.borderWidth);
if (e4.style.position=="relative"||e4.style.position=="absolute")
e4.sizeBar.style.top=(event.clientY-this.GetOffsetTop(e4,e4,document.body)-h7+window.scrollY)+"px";
else 
e4.sizeBar.style.top=(event.clientY-h7+window.scrollY)+"px";
}else {
this.b5.style.height=""+(parseInt(this.b5.style.height)+ag5)+"px";
this.b7=event.clientY;
}
}
}
}else {
this.b8=p1;
if (this.b8==null||this.GetSpread(this.b8)!=e4)return ;
var p1=this.GetSizeColumn(e4,this.b8,event);
if (p1!=null){
this.b4=p1;
this.b8.style.cursor=this.GetResizeCursor(false);
}else {
var p1=this.GetSizeRow(e4,this.b8,event);
if (p1!=null){
this.b5=p1;
if (this.b8!=null&&this.b8.style!=null)this.b8.style.cursor=this.GetResizeCursor(true);
}else {
if (this.b8!=null&&this.b8.style!=null){
var h4=this.GetCell(this.b8);
if (h4!=null&&this.IsHeaderCell(e4,h4)){
if (this.b8.getAttribute("FpSpread")=="rowpadding"||this.b8.getAttribute("ControlType")=="chgrayarea")
this.b8.style.cursor=this.GetgrayAreaCursor(e4);
else 
this.b8.style.cursor="default";
}else {
if (this.b8!=null&&this.b8.style!=null&&(this.b8.getAttribute("FpSpread")=="rowpadding"||this.b8.getAttribute("ControlType")=="chgrayarea"))
this.b8.style.cursor=this.GetgrayAreaCursor(e4);
}
}
}
}
}
}
this.GetgrayAreaCursor=function (e4){
if (e4.c3!=null&&e4.c3.style.cursor!=null){
if (e4.c3.style.cursor=="auto")
e4.c3.style.cursor="default";
return e4.c3.style.cursor;
}
else return "default";
}
this.GetResizeCursor=function (i9){
if (i9){
return "n-resize";
}else {
return "w-resize";
}
}
this.HitCommandBar=function (p1){
var g0=p1;
var e4=this.GetTopSpread(this.GetSpread(g0,true));
if (e4==null)return false;
var r2=this.GetCommandBar(e4);
while (g0!=null&&g0!=e4){
if (g0==r2)return true;
g0=g0.parentNode;
}
return false;
}
this.OpenWaitMsg=function (e4){
var i3=document.getElementById(e4.id+"_waitmsg");
if (i3==null)return ;
var j3=e4.offsetWidth;
var k0=e4.offsetHeight;
var j0=this.CreateTestBox(e4);
j0.style.fontFamily=i3.style.fontFamily;
j0.style.fontSize=i3.style.fontSize;
j0.style.fontWeight=i3.style.fontWeight;
j0.style.fontStyle=i3.style.fontStyle;
j0.innerHTML=i3.innerHTML;
i3.style.width=""+(j0.offsetWidth+2)+"px";
var ad9=Math.max(10,(j3-parseInt(i3.style.width))/2);
var g0=Math.max(10,(k0-parseInt(i3.style.height))/2);
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
ad9+=this.GetOffsetLeft(e4,e4,document.body);
g0+=this.GetOffsetTop(e4,e4,document.body);
}
i3.style.top=""+g0+"px";
i3.style.left=""+ad9+"px";
i3.style.display="block";
}
this.CloseWaitMsg=function (e4){
var i3=document.getElementById(e4.id+"_waitmsg");
if (i3==null)return ;
i3.style.display="none";
this.Focus(e4);
}
this.MouseDown=function (event){
if (window.fpPostOn!=null)return ;
event=this.GetEvent(event);
var p1=this.GetTarget(event);
var e4=this.GetSpread(p1,true);
e4.mouseY=event.clientY;
var ag8=this.GetPageActiveSpread();
if (this.GetViewport(e4)==null)return ;
if (e4!=null&&p1.parentNode!=null&&p1.parentNode.getAttribute("name")==e4.id+"_slideBar"){
if (this.IsChild(p1,this.GetPager1(e4)))
this.activePager=this.GetPager1(e4);
else if (this.IsChild(p1,this.GetPager2(e4)))
this.activePager=this.GetPager2(e4);
if (this.activePager!=null){
var p4=true;
if (this.a7)p4=this.EndEdit(e4);
if (p4){
this.UpdatePostbackData(e4);
this.dragSlideBar=true;
}
}
return this.CancelDefault(event);
}
if (e4!=null)e4.a6=false;
if (this.GetOperationMode(e4)=="ReadOnly")return ;
var j9=false;
if (e4!=null)j9=this.IsXHTML(e4);
if (this.a7&&e4.getAttribute("mcctCellType")!="true"){
var g0=this.GetCell(p1);
if (g0!=e4.d1){
var p4=this.EndEdit();
if (!p4)return ;
}else 
return ;
}
if (p1==this.GetParent(this.GetViewport(e4))){
if (this.GetTopSpread(ag8)!=e4){
this.SetActiveSpread(event);
}
return ;
}
var ag9=(ag8==e4);
this.SetActiveSpread(event);
ag8=this.GetPageActiveSpread();
if (this.HitCommandBar(p1))return ;
if (event.button==2)return ;
if (this.IsChild(p1,this.GetGroupBar(e4))){
var h6=parseInt(p1.id.replace(e4.id+"_group",""));
if (!isNaN(h6)){
this.InitMovingCol(e4,h6,true,p1);
this.a6=true;
e4.dragFromGroupbar=true;
this.CancelDefault(event);
return ;
}
}
if (this.IsInRowEditTemplate(e4,p1)){
return ;
}
this.b4=this.GetSizeColumn(e4,p1,event);
if (this.b4!=null){
this.a6=true;
this.b6=this.b7=event.clientX;
if (this.b4.style!=null)this.b4.style.cursor=this.GetResizeCursor(false);
this.b8=p1;
if (e4.frzRows>0||e4.frzCols>0){
var ah0=this.GetViewport0(e4);
if (ah0==null)ah0=this.GetViewport1(e4);
if (ah0==null)ah0=this.GetViewport(e4);
ah0=ah0.parentNode;
if (this.GetColHeader(e4)!=null)
e4.sizeBar.style.top=""+(this.GetOffsetTop(e4,ah0,e4)-this.GetColHeader(e4).offsetHeight)+"px";
else 
e4.sizeBar.style.top=""+this.GetOffsetTop(e4,ah0,e4)+"px";
var h7=0;
if (!j9)h7+=parseInt(e4.style.borderWidth);
e4.sizeBar.style.left=(this.b6-this.GetOffsetLeft(e4,e4,document.body)-h7+window.scrollX)+"px";
var ah1=0;
if (this.GetViewport0(e4)!=null)ah1=this.GetViewport0(e4).parentNode.offsetHeight;
if (ah1==0&&this.GetViewport1(e4)!=null)ah1=this.GetViewport1(e4).parentNode.offsetHeight;
if (this.GetViewport(e4)!=null)ah1+=this.GetViewport(e4).parentNode.offsetHeight;
if (this.GetColHeader(e4)!=null)ah1+=this.GetColHeader(e4).offsetHeight;
e4.sizeBar.style.height=""+ah1+"px";
e4.sizeBar.style.width="2px";
}
}else {
this.b5=this.GetSizeRow(e4,p1,event);
if (this.b5!=null){
this.a6=true;
this.b6=this.b7=event.clientY;
this.b5.style.cursor=this.GetResizeCursor(true);
this.b8=p1;
e4.a6=true;
if (e4.frzRows>0||e4.frzCols>0){
var ah0=this.GetViewport0(e4);
if (ah0==null)ah0=this.GetViewport1(e4);
if (ah0==null)ah0=this.GetViewport(e4);
ah0=ah0.parentNode;
var h7=0;
if (!j9)h7+=parseInt(e4.style.borderWidth);
if (e4.style.position=="relative"||e4.style.position=="absolute"){
e4.sizeBar.style.left="0px";
e4.sizeBar.style.top=(this.b7-this.GetOffsetTop(e4,e4,document.body)-h7+window.scrollY)+"px";
}else {
e4.sizeBar.style.left=""+this.GetOffsetLeft(e4,e4,document.body)+"px";
e4.sizeBar.style.top=(this.b7-h7+window.scrollY)+"px";
}
e4.sizeBar.style.height="2px";
e4.sizeBar.style.width=""+e4.offsetWidth+"px";
}
}else {
var ah2=this.GetCell(p1,null,event);
if (ah2==null){
var c4=this.GetCorner(e4);
if (c4!=null&&this.IsChild(p1,c4)){
if (this.GetOperationMode(e4)=="Normal")
this.SelectTable(e4,true);
}
return ;
}
var ah3=this.GetColFromCell(e4,ah2);
if (ah2.parentNode.getAttribute("FpSpread")=="ch"&&ah3>=this.GetColCount(e4))return ;
if (ah2.parentNode.getAttribute("FpSpread")=="rh"&&this.IsChildSpreadRow(e4,this.GetViewport(e4),ah2.parentNode.rowIndex))return ;
if (ah2.parentNode.getAttribute("FpSpread")=="ch"&&this.GetOperationMode(e4)!="Normal"){
if (e4.allowColMove||e4.allowGroup){
if (e4.getAttribute("LayoutMode"))ah3=parseInt(ah2.getAttribute("col"));
this.InitMovingCol(e4,ah3);
}
this.a6=true;
this.b8=p1;
return this.CancelDefault(event);
}
if (ah2.parentNode.getAttribute("FpSpread")=="ch"&&(this.GetOperationMode(e4)=="RowMode"||this.GetOperationMode(e4)=="SingleSelect"||this.GetOperationMode(e4)=="ExtendedSelect")){
if (!e4.allowColMove&&!e4.allowGroup)
return ;
}else {
var p2=this.FireActiveCellChangingEvent(e4,this.GetRowFromCell(e4,ah2),ah3,ah2.parentNode.getAttribute("row"));
if (p2)return ;
var o2=this.GetOperationMode(e4);
var e9=this.GetTopSpread(e4);
if (!event.ctrlKey||e4.getAttribute("multiRange")!="true"){
if (o2!="MultiSelect"){
if (!(
(e4.allowColMove||e4.allowGroup)&&ah2.parentNode.getAttribute("FpSpread")=="ch"&&
o2=="Normal"&&(e4.getAttribute("SelectionPolicy")=="Range"||e4.getAttribute("SelectionPolicy")=="MultiRange")&&
e4.selectedCols.length!=0&&this.IsColSelected(e4,ah3)
))
this.ClearSelection(e4);
}
}else {
if (o2!="ExtendedSelect"&&o2!="MultiSelect"){
if (e4.d1!=null)this.PaintSelectedCell(e4,e4.d1,true);
}
}
}
e4.d1=ah2;
var h4=e4.d1;
var aa8=this.GetParent(this.GetViewport(e4));
if (aa8!=null&&!this.IsControl(p1)&&(p1!=null&&p1.tagName!="scrollbar")){
if (this.IsChild(h4,aa8)&&h4.offsetLeft+h4.offsetWidth>aa8.scrollLeft+aa8.clientWidth){
aa8.scrollLeft=h4.offsetLeft+h4.offsetWidth-aa8.clientWidth;
}
if ((this.IsChild(h4,aa8)||this.IsChild(h4,this.GetViewport2(e4)))&&h4.offsetTop+h4.offsetHeight>aa8.scrollTop+aa8.clientHeight&&h4.offsetHeight<aa8.clientHeight){
aa8.scrollTop=h4.offsetTop+h4.offsetHeight-aa8.clientHeight;
}
if (h4.offsetTop<aa8.scrollTop){
aa8.scrollTop=h4.offsetTop;
}
if (h4.offsetLeft<aa8.scrollLeft){
aa8.scrollLeft=h4.offsetLeft;
}
this.ScrollView(e4);
}
if (ah2.parentNode.getAttribute("FpSpread")!="ch")this.SetActiveRow(e4,this.GetRowKeyFromCell(e4,e4.d1));
if (ah2.parentNode.getAttribute("FpSpread")=="rh")
this.SetActiveCol(e4,0);
else {
this.SetActiveCol(e4,e4.getAttribute("LayoutMode")?this.GetColKeyFromCell2(e4,e4.d1):this.GetColKeyFromCell(e4,e4.d1));
}
var o2=this.GetOperationMode(e4);
if (e4.d1.parentNode.getAttribute("FpSpread")=="r"){
if (o2=="ExtendedSelect"||o2=="MultiSelect"){
var ah4=this.IsRowSelected(e4,this.GetRowFromCell(e4,e4.d1));
if (ah4)
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,false,true);
else 
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
}
else if (o2=="RowMode"||o2=="SingleSelect")
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
else {
this.SelectRange(e4,this.GetRowFromCell(e4,e4.d1),ah3,1,1,true);
}
e4.d5=this.GetRowFromCell(e4,e4.d1);
e4.d6=ah3;
}else if (e4.d1.parentNode.getAttribute("FpSpread")=="ch"){
if (p1.tagName=="INPUT"||p1.tagName=="TEXTAREA"||p1.tagName=="SELECT")
return ;
var u6=ah3;
if (e4.allowColMove||e4.allowGroup)
{
if (o2=="Normal"&&(e4.getAttribute("SelectionPolicy")=="Range"||e4.getAttribute("SelectionPolicy")=="MultiRange")){
if (this.IsColSelected(e4,u6))this.InitMovingCol(e4,u6);
else this.SelectColumn(e4,u6,1,true);
}else {
if (e4.getAttribute("LayoutMode"))
u6=parseInt(e4.d1.getAttribute("col"));
if (o2=="Normal"||o2=="ReadOnly")
this.SelectColumn(e4,u6,1,true);
else e4.selectedCols.push(u6);
this.InitMovingCol(e4,u6);
}
}else {
if (o2=="Normal"||o2=="ReadOnly"){
if (e4.getAttribute("LayoutMode"))
u6=parseInt(e4.d1.getAttribute("col"));
this.SelectColumn(e4,u6,1,true);
}
else 
return ;
}
}else if (e4.d1.parentNode.getAttribute("FpSpread")=="rh"){
if (p1.tagName=="INPUT"||p1.tagName=="TEXTAREA"||p1.tagName=="SELECT")
return ;
if (o2=="ExtendedSelect"||o2=="MultiSelect"){
if (this.IsRowSelected(e4,this.GetRowFromCell(e4,e4.d1)))
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,false,true);
else 
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
}else {
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true);
}
}
if (e4.d1!=null){
var g3=this.CreateEvent("ActiveCellChanged");
g3.cmdID=e4.id;
g3.Row=g3.row=this.GetSheetIndex(e4,this.GetRowFromCell(e4,e4.d1));
g3.Col=g3.col=ah3;
if (e4.getAttribute("LayoutMode"))
g3.InnerRow=g3.innerRow=e4.d1.parentNode.getAttribute("row");
this.FireEvent(e4,g3);
}
e4.d2=e4.d1;
if (e4.d1!=null){
e4.d3=this.GetRowFromCell(e4,e4.d1);
e4.d4=ah3;
}
this.b8=p1;
this.a6=true;
}
}
this.EnableButtons(e4);
if (this.dragCol!=null&&this.dragCol>=0&&!this.IsControl(p1))this.Focus(e4);
if (!this.a7&&this.b5==null&&this.b4==null){
if (e4.d1!=null&&this.IsChild(e4.d1,e4)&&!this.IsHeaderCell(this.GetCell(p1))){
var i4=this.GetEditor(e4.d1);
if (i4!=null){
if (i4.type=="submit")this.SaveData(e4);
this.a7=(i4.type!="button"&&i4.type!="submit");
this.a8=i4;
this.a9=this.GetEditorValue(i4);
if (i4.focus)i4.focus();
}
}
}
if (!this.IsControl(p1)){
if (e4!=null)this.UpdatePostbackData(e4);
return this.CancelDefault(event);
}
}
this.GetMovingCol=function (e4){
var z2=document.getElementById(e4.id+"movingCol");
if (z2==null){
z2=document.createElement("DIV");
z2.style.display="none";
z2.style.position="absolute";
z2.style.top="0px";
z2.style.left="0px";
z2.id=e4.id+"movingCol";
z2.align="center";
e4.insertBefore(z2,null);
if (e4.getAttribute("DragColumnCssClass")!=null)
z2.className=e4.getAttribute("DragColumnCssClass");
else 
z2.style.border="1px solid black";
z2.style.MozOpacity=0.50;
}
return z2;
}
this.IsControl=function (g0){
return (g0!=null&&(g0.tagName=="INPUT"||g0.tagName=="TEXTAREA"||g0.tagName=="SELECT"||g0.tagName=="OPTION"));
}
this.EnableButtons=function (e4){
var t1=this.GetCellType(e4.d1);
var o0=this.GetSelection(e4);
var o1=o0.lastChild;
var v8=e4.getAttribute("OperationMode");
var ah5=v8=="ReadOnly"||v8=="SingleSelect"||t1=="readonly";
if (!ah5){
ah5=this.AnyReadOnlyCell(e4,o1);
}
if (ah5){
var f9=this.GetCmdBtn(e4,"Copy");
this.UpdateCmdBtnState(f9,o1==null);
var f6=this.b9;
f9=this.GetCmdBtn(e4,"Paste");
this.UpdateCmdBtnState(f9,(o1==null||f6==null));
f9=this.GetCmdBtn(e4,"Clear");
this.UpdateCmdBtnState(f9,true);
}else {
var f9=this.GetCmdBtn(e4,"Copy");
this.UpdateCmdBtnState(f9,o1==null);
var f6=this.b9;
f9=this.GetCmdBtn(e4,"Paste");
this.UpdateCmdBtnState(f9,(o1==null||f6==null));
f9=this.GetCmdBtn(e4,"Clear");
this.UpdateCmdBtnState(f9,o1==null);
}
}
this.CellClicked=function (h4){
var e4=this.GetSpread(h4);
if (e4!=null){
this.SaveData(e4);
}
}
this.UpdateCmdBtnState=function (f9,disabled){
if (f9==null)return ;
if (f9.tagName=="INPUT"){
var g0=f9.disabled;
if (g0==disabled)return ;
f9.disabled=disabled;
}else {
var g0=f9.getAttribute("disabled");
if (g0==disabled)return ;
f9.setAttribute("disabled",disabled);
}
if (f9.tagName=="IMG"){
var ah6=f9.getAttribute("disabledImg");
if (disabled&&ah6!=null&&ah6!=""){
if (f9.src.indexOf(ah6)<0)f9.src=ah6;
}else {
var ah7=f9.getAttribute("enabledImg");
if (f9.src.indexOf(ah7)<0)f9.src=ah7;
}
}
}
this.MouseUp=function (event){
if (window.fpPostOn!=null)return ;
event=this.GetEvent(event);
var p1=this.GetTarget(event);
var e4=this.GetSpread(p1,true);
if (e4==null&&!this.a6){
return ;
}
if (this.dragSlideBar&&e4!=null)
{
this.dragSlideBar=false;
if (this.activePager!=null){
var af5=this.MoveSliderBar(e4,event)-1;
this.activePager=null;
this.GotoPage(e4,af5);
}
return ;
}
if (this.a6&&(this.b4!=null||this.b5!=null)){
if (this.b4!=null)
e4=this.GetSpread(this.b4);
else 
e4=this.GetSpread(this.b5);
}
if (e4==null)return ;
if (this.GetViewport(e4)==null)return ;
var v8=this.GetOperationMode(e4);
if (v8=="ReadOnly")return ;
var i3=true;
if (this.a6){
this.a6=false;
if (this.dragCol!=null&&this.dragCol>=0){
var ah8=(this.IsChild(p1,this.GetGroupBar(e4))||p1==this.GetGroupBar(e4));
if (!ah8&&this.GetGroupBar(e4)!=null){
var ah9=event.clientX;
var ai0=event.clientY;
var r6=this.GetOffsetLeft(e4,e4,document.body);
var x2=this.GetOffsetTop(e4,e4,document.body);
var ai1=this.GetGroupBar(e4).offsetWidth;
var ai2=this.GetGroupBar(e4).offsetHeight;
var r1=window.scrollX;
var r0=window.scrollY;
var m7=document.getElementById(e4.id+"_titleBar");
if (m7)r0-=m7.parentNode.parentNode.offsetHeight;
if (this.GetPager1(e4)!=null)r0-=this.GetPager1(e4).offsetHeight;
ah8=(r6<=r1+ah9&&r1+ah9<=r6+ai1&&x2<=r0+ai0&&r0+ai0<=x2+ai2);
}
if (e4.dragFromGroupbar){
if (ah8){
if (e4.targetCol>0)
this.Regroup(e4,this.dragCol,parseInt((e4.targetCol+1)/2));
else 
this.Regroup(e4,this.dragCol,e4.targetCol);
}else {
this.Ungroup(e4,this.dragCol,e4.targetCol);
}
}else {
if (ah8){
if (e4.allowGroup){
if (e4.targetCol>0)
this.Group(e4,this.dragCol,parseInt((e4.targetCol+1)/2));
else 
this.Group(e4,this.dragCol,e4.targetCol);
}
}else if (e4.allowColMove){
if (e4.targetCol!=null){
var g3=this.CreateEvent("ColumnDragMove");
g3.cancel=false;
g3.col=e4.selectedCols;
this.FireEvent(e4,g3);
if (!g3.cancel){
this.MoveCol(e4,this.dragCol,e4.targetCol);
var g3=this.CreateEvent("ColumnDragMoveCompleted");
g3.col=e4.selectedCols;
this.FireEvent(e4,g3);
}
}
}
}
var z2=this.GetMovingCol(e4);
if (z2!=null)
z2.style.display="none";
this.dragCol=-1;
this.dragViewCol=-1;
var af6=this.GetPosIndicator(e4);
if (af6!=null)
af6.style.display="none";
e4.dragFromGroupbar=false;
e4.targetCol=null;
this.b4=this.b5=null;
}
if (this.b4!=null){
if (e4.sizeBar!=null)e4.sizeBar.style.left="-400px";
i3=false;
var ag5=event.clientX-this.b6;
var aa2=parseInt(this.b4.width);
var ai3=aa2;
if (isNaN(aa2))aa2=0;
aa2+=ag5;
if (aa2<1)aa2=1;
var k4=parseInt(this.b4.getAttribute("index"));
var v0=this.GetColGroup(this.GetViewport(e4));
if (this.IsChild(this.b4,this.GetFrozColHeader(e4))){
v0=this.GetColGroup(this.GetViewport0(e4));
if (v0==null)v0=this.GetColGroup(this.GetViewport2(e4));
}
if (v0!=null&&v0.childNodes.length>0){
if (this.IsChild(this.b4,this.GetColHeader(e4)))
ai3=parseInt(v0.childNodes[k4-e4.frzCols].width);
else 
ai3=parseInt(v0.childNodes[k4].width);
}else {
ai3=1;
}
if (this.GetViewport(e4).rules!="rows"){
if (k4==parseInt(this.colCount)-1)ai3-=1;
}
if (aa2!=ai3&&event.clientX!=this.b7)
{
this.SetColWidth(e4,k4,aa2,ai3);
var g3=this.CreateEvent("ColWidthChanged");
g3.col=k4;
g3.width=aa2;
this.FireEvent(e4,g3);
}
this.ScrollView(e4);
this.PaintFocusRect(e4);
}else if (this.b5!=null){
if (e4.sizeBar!=null){e4.sizeBar.style.left="-400px";e4.sizeBar.style.width="2px";}
i3=false;
var ag5=event.clientY-this.b7;
var ag7=this.b5.offsetHeight+ag5;
if (ag7<1){
ag7=1;
ag5=1-this.b5.offsetHeight;
}
this.b5.style.height=""+ag7+"px";
this.b5.style.cursor="auto";
var i6=null;
if (this.IsChild(this.b5,this.GetFrozRowHeader(e4))){
i6=this.GetViewport1(e4);
}else {
i6=this.GetViewport(e4);
}
if (i6.rows.length>=2&&i6.cellSpacing=="0"&&e4.frzRow==0){
if (this.b5.rowIndex==0)
i6.rows[0].style.height=""+(parseInt(this.b5.style.height)-1)+"px";
else 
if (this.b5.rowIndex==i6.rows.length-1)
i6.rows[this.b5.rowIndex].style.height=""+(parseInt(this.b5.style.height)+1)+"px";
else 
i6.rows[this.b5.rowIndex].style.height=this.b5.style.height;
}else {
i6.rows[this.b5.rowIndex].style.height=""+(this.b5.offsetHeight-i6.rows[0].offsetTop)+"px";
}
var ai4=this.GetViewport2(e4);
if (this.IsChild(this.b5,this.GetFrozRowHeader(e4))){
ai4=this.GetViewport0(e4);
}
if (ai4!=null)
ai4.rows[this.b5.rowIndex].style.height=i6.rows[this.b5.rowIndex].style.height;
if (this.IsChild(this.b5,this.GetFrozRowHeader(e4))){
this.GetFrozRowHeader(e4).parentNode.parentNode.parentNode.style.posHeight+=ag5;
}
var r5=this.AddRowInfo(e4,this.b5.getAttribute("FpKey"));
if (r5!=null){
this.SetRowHeight(e4,r5,parseInt(this.b5.style.height));
}
if (this.b6!=event.clientY){
var g3=this.CreateEvent("RowHeightChanged");
g3.row=this.GetRowFromCell(e4,this.b5.cells[0]);
g3.height=this.b5.offsetHeight;
this.FireEvent(e4,g3);
}
var i8=this.GetParentSpread(e4);
if (i8!=null)this.UpdateRowHeight(i8,e4);
var e9=this.GetTopSpread(e4);
this.SizeAll(e9);
this.Refresh(e9);
this.ScrollView(e4);
this.PaintFocusRect(e4);
}else {
}
if (this.b8!=null){
this.b8=null;
}
}
if (i3)i3=!this.IsControl(p1);
if (i3&&this.HitCommandBar(p1))return ;
var ai5=false;
var o0=this.GetSelection(e4);
if (o0!=null){
var o1=o0.firstChild;
var h9=new this.Range();
if (o1!=null){
h9.row=this.GetRowByKey(e4,o1.getAttribute("row"));
h9.col=this.GetColByKey(e4,o1.getAttribute("col"));
h9.rowCount=parseInt(o1.getAttribute("rowcount"));
h9.colCount=parseInt(o1.getAttribute("colcount"));
}
switch (e4.d7){
case "":
var g9=this.GetViewport(e4).rows;
for (var f1=h9.row;f1<h9.row+h9.rowCount&&f1<g9.length;f1++){
if (g9[f1].cells.length>0&&g9[f1].cells[0].firstChild!=null&&g9[f1].cells[0].firstChild.nodeName!="#text"){
if (g9[f1].cells[0].firstChild.getAttribute("FpSpread")=="Spread"){
ai5=true;
break ;
}
}
}
break ;
case "c":
var i6=this.GetViewport(e4);
for (var f1=0;f1<i6.rows.length;f1++){
if (this.IsChildSpreadRow(e4,i6,f1)){
ai5=true;
break ;
}
}
break ;
case "r":
var i6=this.GetViewport(e4);
var u5=h9.rowCount;
for (var f1=h9.row;f1<h9.row+u5&&f1<i6.rows.length;f1++){
if (this.IsChildSpreadRow(e4,i6,f1)){
ai5=true;
break ;
}
}
}
}
if (ai5){
var f9=this.GetCmdBtn(e4,"Copy");
this.UpdateCmdBtnState(f9,true);
f9=this.GetCmdBtn(e4,"Paste");
this.UpdateCmdBtnState(f9,true);
f9=this.GetCmdBtn(e4,"Clear");
this.UpdateCmdBtnState(f9,true);
}
var e9=this.GetTopSpread(e4);
if (e9.style.position!="absolute"&&e9.style.position!="relative"){
var g1=document.getElementById(e9.id+"_textBox");
if (g1!=null){
g1.style.top=""+(window.scrollY+event.clientY)+"px";;
g1.style.left=""+(window.scrollX+event.clientX)+"px";
}
}
if (i3)this.Focus(e4);
}
this.UpdateRowHeight=function (i8,child){
var m9=child.parentNode;
while (m9!=null){
if (m9.tagName=="TR")break ;
m9=m9.parentNode;
}
var j9=this.IsXHTML(i8);
if (m9!=null){
var f0=m9.rowIndex;
if (this.GetRowHeader(i8)!=null){
var r3=0;
if (this.GetColHeader(child)!=null)r3=this.GetColHeader(child).offsetHeight;
if (this.GetRowHeader(child)!=null)r3+=this.GetRowHeader(child).offsetHeight;
if (!j9)r3-=this.GetViewport(i8).cellSpacing;
if (this.GetViewport(i8).cellSpacing==0){
this.GetRowHeader(i8).rows[f0].style.height=""+(r3+1)+"px";
if (this.GetParentSpread(i8)!=null){
this.GetRowHeader(i8).parentNode.style.height=""+this.GetRowHeader(i8).offsetHeight+"px";
}
}
else 
this.GetRowHeader(i8).rows[f0].style.height=""+(r3+2)+"px";
this.GetViewport(i8).rows[f0].style.height=""+r3+"px";
child.style.height=""+r3+"px";
}
}
var ai6=this.GetParentSpread(i8);
if (ai6!=null)
this.UpdateRowHeight(ai6,i8);
}
this.MouseOut=function (){
if (!this.a6&&this.b4!=null&&this.b4.style!=null)this.b4.style.cursor="auto";
}
this.KeyDown=function (e4,event){
if (window.fpPostOn!=null)return ;
if (!e4.ProcessKeyMap(event))return ;
if (event.keyCode==event.DOM_VK_SPACE&&e4.d1!=null){
var o2=this.GetOperationMode(e4);
if (o2=="MultiSelect"){
if (this.IsRowSelected(e4,this.GetRowFromCell(e4,e4.d1)))
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,false,true);
else 
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
return ;
}
}
var i4=false;
if (this.a7&&this.a8!=null){
var ai7=this.GetEditor(this.a8);
i4=(ai7!=null);
}
if (event.keyCode!=event.DOM_VK_LEFT&&event.keyCode!=event.DOM_VK_RIGHT&&event.keyCode!=event.DOM_VK_RETURN&&event.keyCode!=event.DOM_VK_TAB&&(this.a7&&!i4)&&this.a8.tagName=="SELECT")return ;
if (this.a7&&this.a8!=null&&this.a8.getAttribute("MccbId")){
var ai8=eval(this.a8.getAttribute("MccbId")+"_Obj");
if (event.altKey&&event.keyCode==event.DOM_VK_DOWN)return ;
if (ai8!=null&&ai8.getIsDrop!=null&&ai8.getIsDrop())return ;
}
switch (event.keyCode){
case event.DOM_VK_LEFT:
case event.DOM_VK_RIGHT:
if (i4){
var ai9=this.a8.getAttribute("FpEditor");
if (this.a7&&ai9=="ExtenderEditor"){
var aj0=FpExtender.Util.getEditor(this.a8);
if (aj0&&aj0.type!="text")this.EndEdit();
}
if (ai9!="RadioButton"&&ai9!="ExtenderEditor")this.EndEdit();
}
if (!this.a7){
this.NextCell(e4,event,event.keyCode);
}
break ;
case event.DOM_VK_UP:
case event.DOM_VK_DOWN:
case event.DOM_VK_RETURN:
if (this.a8!=null&&this.a8.tagName=="TEXTAREA")return ;
if (/*event.keyCode!=event.DOM_VK_RETURN&&*/i4&&this.a7&&this.a8.getAttribute("FpEditor")=="ExtenderEditor"){
var aj1=this.a8.getAttribute("Extenders");
if (aj1&&aj1.indexOf("AutoCompleteExtender")!=-1)return ;
}
if (event.keyCode==event.DOM_VK_RETURN)this.CancelDefault(event);
if (this.a7){
var p4=this.EndEdit();
if (!p4)return ;
}
if (event.keyCode!=event.DOM_VK_RETURN)this.NextCell(e4,event,event.keyCode);
var e9=this.GetTopSpread(e4);
var g1=document.getElementById(e9.id+"_textBox");
if (event.DOM_VK_RETURN==event.keyCode)g1.focus();
break ;
case event.DOM_VK_TAB:
if (this.a7){
var p4=this.EndEdit();
if (!p4)return ;
}
var p3=this.GetProcessTab(e4);
var aj2=(p3=="true"||p3=="True");
if (aj2)this.NextCell(e4,event,event.keyCode);
break ;
case event.DOM_VK_SHIFT:
break ;
case event.DOM_VK_HOME:
case event.DOM_VK_END:
case event.DOM_VK_PAGE_UP:
case event.DOM_VK_PAGE_DOWN:
if (!this.a7){
this.NextCell(e4,event,event.keyCode);
}
break ;
default :
var e5=window.navigator.userAgent;
var y0=(e5.indexOf("Firefox/2.")>=0);
if (y0){
if (event.keyCode==67&&event.ctrlKey&&(!this.a7||i4))this.Copy(e4);
else if (event.keyCode==86&&event.ctrlKey&&(!this.a7||i4))this.Paste(e4);
else if (event.keyCode==88&&event.ctrlKey&&(!this.a7||i4))this.Clear(e4);
else if (!this.a7&&e4.d1!=null&&!this.IsHeaderCell(e4.d1)&&!event.ctrlKey&&!event.altKey){
this.StartEdit(e4,e4.d1);
}
}else {
if (event.charCode==99&&event.ctrlKey&&(!this.a7||i4))this.Copy(e4);
else if (event.charCode==118&&event.ctrlKey&&(!this.a7||i4))this.Paste(e4);
else if (event.charCode==120&&event.ctrlKey&&(!this.a7||i4))this.Clear(e4);
else if (!this.a7&&e4.d1!=null&&!this.IsHeaderCell(e4.d1)&&!event.ctrlKey&&!event.altKey){
this.StartEdit(e4,e4.d1);
}
}
break ;
}
}
this.GetProcessTab=function (e4){
return e4.getAttribute("ProcessTab");
}
this.ExpandRow=function (e4,i9){
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"ExpandView,"+i9,e4);
else 
__doPostBack(v6,"ExpandView,"+i9);
}
this.SortColumn=function (e4,column,t3){
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"SortColumn,"+column,e4);
else 
__doPostBack(v6,"SortColumn,"+column);
}
this.Filter=function (event,e4){
var p1=this.GetTarget(event);
var g0=p1.value;
if (p1.tagName=="SELECT"){
var ac0=new RegExp("\\s*");
var aj3=new RegExp("\\S*");
var v9=p1[p1.selectedIndex].text;
var aj4="";
var f1=0;
var f0=g0.length;
while (f0>0){
var h5=g0.match(ac0);
if (h5!=null){
aj4+=h5[0];
f1=h5[0].length;
f0-=f1;
g0=g0.substring(f1);
h5=g0.match(aj3);
if (h5!=null){
f1=h5[0].length;
f0-=f1;
g0=g0.substring(f1);
}
}else {
break ;
}
h5=v9.match(aj3);
if (h5!=null){
aj4+=h5[0];
f1=h5[0].length;
v9=v9.substring(f1);
h5=v9.match(ac0);
if (h5!=null){
f1=h5[0].length;
v9=v9.substring(f1);
}
}else {
break ;
}
}
g0=aj4;
}
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1){
this.SyncData(p1.name,g0,e4);
e4.LoadState=null;
}
else 
__doPostBack(p1.name,g0);
}
this.MoveCol=function (e4,from,to){
var v6=e4.getAttribute("name");
if (e4.selectedCols&&e4.selectedCols.length>0){
var aj5=[];
for (var f1=0;f1<e4.selectedCols.length;f1++)
aj5[f1]=this.GetSheetColIndex(e4,e4.selectedCols[f1]);
var aj6=aj5.join("+");
this.MoveMultiCol(e4,aj6,to);
return ;
}
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"MoveCol,"+from+","+to,e4);
else 
__doPostBack(v6,"MoveCol,"+from+","+to);
}
this.MoveMultiCol=function (e4,aj6,to){
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"MoveCol,"+aj6+","+to,e4);
else 
__doPostBack(v6,"MoveCol,"+aj6+","+to);
}
this.Group=function (e4,n9,toCol){
var v6=e4.getAttribute("name");
if (e4.selectedCols&&e4.selectedCols.length>0){
var aj5=[];
for (var f1=0;f1<e4.selectedCols.length;f1++)
if (e4.getAttribute("LayoutMode"))
aj5[f1]=parseInt(e4.selectedCols[f1]);
else 
aj5[f1]=this.GetSheetColIndex(e4,e4.selectedCols[f1]);
var aj6=aj5.join("+");
this.GroupMultiCol(e4,aj6,toCol);
e4.selectedCols=[];
return ;
}
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Group,"+n9+","+toCol,e4);
else 
__doPostBack(v6,"Group,"+n9+","+toCol);
}
this.GroupMultiCol=function (e4,aj6,toCol){
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Group,"+aj6+","+toCol,e4);
else 
__doPostBack(v6,"Group,"+aj6+","+toCol);
}
this.Ungroup=function (e4,n9,toCol){
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Ungroup,"+n9+","+toCol,e4);
else 
__doPostBack(v6,"Ungroup,"+n9+","+toCol);
}
this.Regroup=function (e4,fromCol,toCol){
var v6=e4.getAttribute("name");
var ac1=(e4.getAttribute("ajax")!="false");
if (ac1)
this.SyncData(v6,"Regroup,"+fromCol+","+toCol,e4);
else 
__doPostBack(v6,"Regroup,"+fromCol+","+toCol);
}
this.ProcessData=function (){
try {
var aj7=this;
aj7.removeEventListener("load",the_fpSpread.ProcessData,false);
var p1=window.srcfpspread;
p1=p1.split(":").join("_");
var aj8=window.fpcommand;
var aj9=document;
var ak0=aj9.getElementById(p1+"_buff");
if (ak0==null){
ak0=aj9.createElement("iframe");
ak0.id=p1+"_buff";
ak0.style.display="none";
aj9.body.appendChild(ak0);
}
var e4=aj9.getElementById(p1);
the_fpSpread.CloseWaitMsg(e4);
if (ak0==null)return ;
var ak1=aj7.responseText;
ak0.contentWindow.document.body.innerHTML=ak1;
var p3=ak0.contentWindow.document.getElementById(p1+"_values");
if (p3!=null){
var v4=p3.getElementsByTagName("data")[0];
var o1=v4.firstChild;
the_fpSpread.error=false;
while (o1!=null){
var h1=the_fpSpread.GetRowByKey(e4,o1.getAttribute("r"));
var h3=the_fpSpread.GetColByKey(e4,o1.getAttribute("c"));
var ac3=the_fpSpread.GetValue(e4,h1,h3);
if (o1.innerHTML!=ac3){
var i3=the_fpSpread.GetFormula(e4,h1,h3);
var j2=the_fpSpread.GetCellByRowCol(e4,h1,h3);
the_fpSpread.SetCellValueFromView(j2,o1.innerHTML,true);
j2.setAttribute("FpFormula",i3);
}
o1=o1.nextSibling;
}
the_fpSpread.ClearCellData(e4);
}else {
the_fpSpread.UpdateSpread(aj9,ak0,p1,ak1,aj8);
}
var ac2=the_fpSpread.GetForm(e4);
ac2.__EVENTTARGET.value="";
ac2.__EVENTARGUMENT.value="";
var ac3=aj9.getElementsByName("__VIEWSTATE")[0];
var g0=ak0.contentWindow.document.getElementsByName("__VIEWSTATE")[0];
if (ac3!=null&&g0!=null)ac3.value=g0.value;
ac3=aj9.getElementsByName("__EVENTVALIDATION");
g0=ak0.contentWindow.document.getElementsByName("__EVENTVALIDATION");
if (ac3!=null&&g0!=null&&ac3.length>0&&g0.length>0)
ac3[0].value=g0[0].value;
ak0.contentWindow.document.location="about:blank";
window.fpPostOn=null;
d8=null;
}catch (g3){
window.fpPostOn=null;
d8=null;
}
var e4=the_fpSpread.GetTopSpread(aj9.getElementById(p1));
var g3=the_fpSpread.CreateEvent("CallBackStopped");
g3.command=aj8;
the_fpSpread.FireEvent(e4,g3);
};
this.UpdateSpread=function (aj9,ak0,p1,ak1,aj8){
var e4=the_fpSpread.GetTopSpread(aj9.getElementById(p1));
var t2=ak0.contentWindow.document.getElementById(e4.id);
if (t2!=null){
if (typeof(Sys)!=='undefined'){
FarPoint.System.ExtenderHelper.saveLoadedExtenderScripts(e4);
}
the_fpSpread.error=(t2.getAttribute("error")=="true");
if (aj8=="LoadOnDemand"&&!the_fpSpread.error){
var ak2=this.GetElementById(e4,e4.id+"_data");
var ak3=this.GetElementById(t2,e4.id+"_data");
if (ak2!=null&&ak3!=null)ak2.setAttribute("data",ak3.getAttribute("data"));
var ak4=t2.getElementsByTagName("style");
if (ak4!=null){
for (var f1=0;f1<ak4.length;f1++){
if (ak4[f1]!=null&&ak4[f1].innerHTML!=null&&ak4[f1].innerHTML.indexOf(e4.id+"msgStyle")<0)
e4.appendChild(ak4[f1].cloneNode(true));
}
}
var ak5=this.GetElementById(e4,e4.id+"_LoadInfo");
var ak6=this.GetElementById(t2,e4.id+"_LoadInfo");
if (ak5!=null&&ak6!=null)ak5.value=ak6.value;
var ak7=false;
var ak8=this.GetElementById(t2,e4.id+"_rowHeader");
if (ak8!=null){
ak8=ak8.firstChild;
ak7=(ak8.rows.length>1);
var j8=this.GetRowHeader(e4);
this.LoadRows(j8,ak8,true);
}
var ak9=this.GetElementById(t2,e4.id+"_viewport2");
if (ak9!=null){
ak7=(ak9.rows.length>0);
var e7=this.GetViewport2(e4);
this.LoadRows(e7,ak9,false);
}
ak9=this.GetElementById(t2,e4.id+"_viewport");
if (ak9!=null){
ak7=(ak9.rows.length>0);
var e7=this.GetViewport(e4);
this.LoadRows(e7,ak9,false);
}
the_fpSpread.Init(e4);
the_fpSpread.LoadScrollbarState(e4);
the_fpSpread.Focus(e4);
if (ak7)
e4.LoadState=null;
else 
e4.LoadState="complete";
if (typeof(Sys)!=='undefined'){
FarPoint.System.ExtenderHelper.loadExtenderScripts(e4,ak0.contentWindow.document);
}
}else {
e4.innerHTML=t2.innerHTML;
the_fpSpread.CopySpreadAttrs(t2,e4);
if (typeof(Sys)!=='undefined'){
FarPoint.System.ExtenderHelper.loadExtenderScripts(e4,ak0.contentWindow.document);
}
var al0=ak0.contentWindow.document.getElementById(e4.id+"_initScript");
eval(al0.value);
}
}else {
the_fpSpread.error=true;
var al1=e4.getAttribute("errorPage");
if (al1!=null&&al1.length>0){
window.location.href=al1;
}
}
}
this.LoadRows=function (e7,ak9,isHeader){
if (e7==null||ak9==null)return ;
var al2=e7.tBodies[0];
var u5=ak9.rows.length;
var al3=null;
if (isHeader){
u5--;
if (al2.rows.length>0)al3=al2.rows[al2.rows.length-1];
}
for (var f1=0;f1<u5;f1++){
var al4=ak9.rows[f1].cloneNode(false);
al2.insertBefore(al4,al3);
al4.innerHTML=ak9.rows[f1].innerHTML;
}
if (!isHeader){
for (var f1=0;f1<ak9.parentNode.childNodes.length;f1++){
var ab8=ak9.parentNode.childNodes[f1];
if (ab8!=ak9){
e7.parentNode.insertBefore(ab8.cloneNode(true),null);
}
}
}
}
this.CopySpreadAttr=function (u1,dest,attrName){
var al5=u1.getAttribute(attrName);
var al6=dest.getAttribute(attrName);
if (al5!=null||al6!=null){
if (al5==null)
dest.removeAttribute(attrName);
else 
dest.setAttribute(attrName,al5);
}
}
this.CopySpreadAttrs=function (u1,dest){
this.CopySpreadAttr(u1,dest,"totalRowCount");
this.CopySpreadAttr(u1,dest,"pageCount");
this.CopySpreadAttr(u1,dest,"loadOnDemand");
this.CopySpreadAttr(u1,dest,"allowGroup");
this.CopySpreadAttr(u1,dest,"colMove");
this.CopySpreadAttr(u1,dest,"showFocusRect");
this.CopySpreadAttr(u1,dest,"FocusBorderColor");
this.CopySpreadAttr(u1,dest,"FocusBorderStyle");
this.CopySpreadAttr(u1,dest,"FpDefaultEditorID");
this.CopySpreadAttr(u1,dest,"hierView");
this.CopySpreadAttr(u1,dest,"IsNewRow");
this.CopySpreadAttr(u1,dest,"cmdTop");
this.CopySpreadAttr(u1,dest,"ProcessTab");
this.CopySpreadAttr(u1,dest,"AcceptFormula");
this.CopySpreadAttr(u1,dest,"EditMode");
this.CopySpreadAttr(u1,dest,"AllowInsert");
this.CopySpreadAttr(u1,dest,"AllowDelete");
this.CopySpreadAttr(u1,dest,"error");
this.CopySpreadAttr(u1,dest,"ajax");
this.CopySpreadAttr(u1,dest,"autoCalc");
this.CopySpreadAttr(u1,dest,"multiRange");
this.CopySpreadAttr(u1,dest,"rowFilter");
this.CopySpreadAttr(u1,dest,"OperationMode");
this.CopySpreadAttr(u1,dest,"selectedForeColor");
this.CopySpreadAttr(u1,dest,"selectedBackColor");
this.CopySpreadAttr(u1,dest,"anchorBackColor");
this.CopySpreadAttr(u1,dest,"columnHeaderAutoTextIndex");
this.CopySpreadAttr(u1,dest,"EnableRowEditTemplate");
this.CopySpreadAttr(u1,dest,"scrollContent");
this.CopySpreadAttr(u1,dest,"scrollContentColumns");
this.CopySpreadAttr(u1,dest,"scrollContentTime");
this.CopySpreadAttr(u1,dest,"scrollContentMaxHeight");
this.CopySpreadAttr(u1,dest,"SelectionPolicy");
this.CopySpreadAttr(u1,dest,"ShowHeaderSelection");
this.CopySpreadAttr(u1,dest,"layoutMode");
this.CopySpreadAttr(u1,dest,"layoutRowCount");
dest.tabIndex=u1.tabIndex;
if (dest.style!=null&&u1.style!=null){
if (dest.style.width!=u1.style.width)dest.style.width=u1.style.width;
if (dest.style.height!=u1.style.height)dest.style.height=u1.style.height;
if (dest.style.border!=u1.style.border)dest.style.border=u1.style.border;
}
}
this.Clone=function (m6){
var g0=document.createElement(m6.tagName);
g0.id=m6.id;
var h3=m6.firstChild;
while (h3!=null){
var q1=this.Clone(h3);
g0.appendChild(q1);
h3=h3.nextSibling;
}
return g0;
}
this.FireEvent=function (e4,g3){
if (e4==null||g3==null)return ;
var e9=this.GetTopSpread(e4);
if (e9!=null){
g3.spread=e4;
e9.dispatchEvent(g3);
}
}
this.GetForm=function (e4)
{
var i3=e4.parentNode;
while (i3!=null&&i3.tagName!="FORM")i3=i3.parentNode;
return i3;
}
this.SyncData=function (v6,aj8,e4,asyncCallBack){
if (window.fpPostOn!=null){
return ;
}
this.a7=false;
var g3=this.CreateEvent("CallBackStart");
g3.cancel=false;
g3.command=aj8;
if (asyncCallBack==null)asyncCallBack=false;
g3.async=asyncCallBack;
if (e4==null){
var q1=v6.split(":").join("_");
e4=document.getElementById(q1);
}
if (e4!=null){
var e9=this.GetTopSpread(e4);
this.FireEvent(e4,g3);
}
if (g3.cancel){
the_fpSpread.ClearCellData(e4);
return ;
}
if (aj8!=null&&(aj8.indexOf("SelectView,")==0||aj8=="Next"||aj8=="Prev"||aj8.indexOf("Group,")==0||aj8.indexOf("Page,")==0))
e4.LoadState=null;
var al7=g3.async;
if (al7){
this.OpenWaitMsg(e4);
}
window.fpPostOn=true;
if (this.error)aj8="update";
try {
var ac2=this.GetForm(e4);
if (ac2==null)return ;
ac2.__EVENTTARGET.value=v6;
ac2.__EVENTARGUMENT.value=encodeURIComponent(aj8);
var al8=ac2.action;
var g0;
if (al8.indexOf("?")>-1){
g0="&";
}
else 
{
g0="?";
}
al8=al8+g0;
var f6=this.CollectData(e4);
var ak1="";
var aj7=(window.XMLHttpRequest)?new XMLHttpRequest():new ActiveXObject("Microsoft.XMLHTTP");
if (aj7==null)return ;
aj7.open("POST",al8,al7);
aj7.setRequestHeader("Content-Type","application/x-www-form-urlencoded");
if (e4!=null)
window.srcfpspread=e4.id;
else 
window.srcfpspread=v6;
window.fpcommand=aj8;
this.AttachEvent(aj7,"load",the_fpSpread.ProcessData,false);
aj7.send(f6);
}catch (g3){
window.fpPostOn=false;
d8=null;
}
};
this.CollectData=function (e4){
var ac2=this.GetForm(e4);
var g0;
var g5="fpcallback=true&";
for (var f1=0;f1<ac2.elements.length;f1++){
g0=ac2.elements[f1];
var al9=g0.tagName.toLowerCase();
if (al9=="input"){
var am0=g0.type;
if (am0=="hidden"||am0=="text"||am0=="password"||((am0=="checkbox"||am0=="radio")&&g0.checked)){
g5+=(g0.name+"="+encodeURIComponent(g0.value)+"&");
}
}else if (al9=="select"){
if (g0.childNodes!=null){
for (var i1=0;i1<g0.childNodes.length;i1++){
var r4=g0.childNodes[i1];
if (r4!=null&&r4.tagName!=null&&r4.tagName.toLowerCase()=="option"&&r4.selected){
g5+=(g0.name+"="+encodeURIComponent(r4.value)+"&");
}
}
}
}else if (al9=="textarea"){
g5+=(g0.name+"="+encodeURIComponent(g0.value)+"&");
}
}
return g5;
};
this.ClearCellData=function (e4){
var f6=this.GetData(e4);
var am1=f6.getElementsByTagName("root")[0];
var f7=am1.getElementsByTagName("data")[0];
if (f7==null)return null;
if (e4.d8!=null){
var i9=e4.d8.firstChild;
while (i9!=null){
var h1=i9.getAttribute("key");
var am2=i9.firstChild;
while (am2!=null){
var h3=am2.getAttribute("key");
var am3=f7.firstChild;
while (am3!=null){
var h5=am3.getAttribute("key");
if (h1==h5){
var am4=false;
var am5=am3.firstChild;
while (am5!=null){
var h6=am5.getAttribute("key");
if (h3==h6){
am3.removeChild(am5);
am4=true;
break ;
}
am5=am5.nextSibling;
}
if (am4)break ;
}
am3=am3.nextSibling;
}
am2=am2.nextSibling;
}
i9=i9.nextSibling;
}
}
e4.d8=null;
var f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null)
this.UpdateCmdBtnState(f9,true);
}
this.StorePostData=function (e4){
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
var af0=f7.getElementsByTagName("data")[0];
if (af0!=null)e4.d8=af0.cloneNode(true);
}
this.ShowMessage=function (e4,x7,i9,n9,time){
var n6=e4.GetRowCount();
var h0=e4.GetColCount();
if (i9==null||n9==null||i9<0||i9>=n6||n9<0||n9>=h0){
i9=-1;
n9=-1;
}
this.ShowMessageInner(e4,x7,i9,n9,time);
}
this.HideMessage=function (e4,i9,n9){
var n6=e4.GetRowCount();
var h0=e4.GetColCount();
if (i9==null||n9==null||i9<0||i9>=n6||n9<0||n9>=h0)
if (e4.msgList&&e4.msgList.centerMsg&&e4.msgList.centerMsg.msgBox.IsVisible)
e4.msgList.centerMsg.msgBox.Hide();
var am6=this.GetMsgObj(e4,i9,n9);
if (am6&&am6.msgBox.IsVisible){
am6.msgBox.Hide();
}
}
this.ShowMessageInner=function (e4,x7,i9,n9,time){
var am6=this.GetMsgObj(e4,i9,n9);
if (am6){
if (am6.timer)
am6.msgBox.Hide();
}
else 
am6=this.CreateMsgObj(e4,i9,n9);
var am7=am6.msgBox;
am7.Show(e4,this,x7);
if (time&&time>0)
am6.timer=setTimeout(function (){am7.Hide();},time);
this.SetMsgObj(e4,am6);
}
this.GetMsgObj=function (e4,i9,n9){
var am6;
var am8=e4.msgList;
if (am8){
if (i9==-1&&n9==-1)
am6=am8.centerMsg;
else if (i9==-2)
am6=am8.hScrollMsg;
else if (n9==-2)
am6=am8.vScrollMsg;
else {
if (am8[i9])
am6=am8[i9][n9];
}
}
return am6;
}
this.SetMsgObj=function (e4,am6){
var am8=e4.msgList;
if (am6.row==-1&&am6.col==-1)
am8.centerMsg=am6;
else if (am6.row==-2)
am8.hScrollMsg=am6;
else if (am6.col==-2)
am8.vScrollMsg=am6;
else {
if (!am8[am6.row])am8[am6.row]=new Array();
am8[am6.row][am6.col]=am6;
}
}
var am9=null;
this.CreateMsgObj=function (e4,i9,n9){
var am7=document.createElement("div");
var am6={row:i9,col:n9,msgBox:am7};
var an0=null;
if (i9!=-2&&n9!=-2){
am7.style.border="1px solid black";
am7.style.background="yellow";
am7.style.color="red";
}
else {
am7.style.border="1px solid #55678e";
am7.style.fontSize="small";
am7.style.background="#E6E9ED";
am7.style.color="#4c5b7f";
this.GetScrollingContentStyle(e4);
an0=am9;
}
if (an0!=null){
if (an0.fontFamily!=null)
am7.style.fontFamily=an0.fontFamily;
if (an0.fontSize!=null)
am7.style.fontSize=an0.fontSize;
if (an0.fontStyle!=null)
am7.style.fontStyle=an0.fontStyle;
if (an0.fontVariant!=null)
am7.style.fontVariant=an0.fontVariant;
if (an0.fontWeight!=null)
am7.style.fontWeight=an0.fontWeight;
if (an0.backgroundColor!=null)
am7.style.backgroundColor=an0.backgroundColor;
if (an0.color!=null)
am7.style.color=an0.color;
}
am7.style.position="absolute";
am7.style.overflow="hidden";
am7.style.display="block";
am7.style.marginLeft=0;
am7.style.marginTop=2;
am7.style.marginRight=0;
am7.style.marginBottom=0;
am7.msgObj=am6;
am7.Show=function (t2,fpObj,x7){
var z0=fpObj.GetMsgPos(t2,this.msgObj.row,this.msgObj.col);
var e8=fpObj.GetCommandBar(t2);
var an1=fpObj.GetGroupBar(t2);
this.style.visibility="visible";
this.style.display="block";
if (x7){
this.style.left=""+0+"px";
this.style.top=""+0+"px";
this.style.width="auto";
this.innerHTML=x7;
}
var n5=fpObj.GetViewport0(t2);
var f4=fpObj.GetViewport1(t2);
var y0=fpObj.GetViewport2(t2);
var an2=(n5||f4||y0);
var an3=(t2.style.position=="relative"||t2.style.position=="absolute");
var an4=z0.top;
var an5=z0.left;
var r4=e4.offsetParent;
while ((r4.tagName=="TD"||r4.tagName=="TR"||r4.tagName=="TBODY"||r4.tagName=="TABLE")&&r4.style.position!="relative"&&r4.style.position!="absolute")
r4=r4.offsetParent;
if (this.msgObj.row>=0&&this.msgObj.col>=0){
if (!an3&&an2&&r4){
var an6=fpObj.GetLocation(t2);
var an7=fpObj.GetLocation(r4);
an4+=an6.y-an7.y;
an5+=an6.x-an7.x;
if (r4.tagName!=="BODY"){
an4-=fpObj.GetBorderWidth(r4,0);
an5-=fpObj.GetBorderWidth(r4,3);
}
}
var an8=fpObj.GetViewPortByRowCol(t2,this.msgObj.row,this.msgObj.col);
if (!this.parentNode&&an8&&an8.parentNode)an8.parentNode.insertBefore(am7,null);
var j3=this.offsetWidth;
this.style.left=""+an5+"px";
if (!an2&&an8&&an8.parentNode&&an5+j3>an8.offsetWidth)
this.style.width=""+(z0.a5-2)+"px";
else if (parseInt(this.style.width)!=j3)
this.style.width=""+(j3-2)+"px";
if (!an2&&an8!=null&&an4>=an8.offsetHeight-2)an4-=z0.a4+this.offsetHeight;
this.style.top=""+an4+"px";
}
else {
if (!an3&&r4){
var an6=fpObj.GetLocation(t2);
var an7=fpObj.GetLocation(r4);
an4+=an6.y-an7.y;
an5+=an6.x-an7.x;
if (r4.tagName!=="BODY"){
an4-=fpObj.GetBorderWidth(r4,0);
an5-=fpObj.GetBorderWidth(r4,3);
}
}
var an9=20;
if (!this.parentNode)t2.insertBefore(am7,null);
if (this.offsetWidth+an9<t2.offsetWidth)
an5+=(t2.offsetWidth-this.offsetWidth-an9)/(this.msgObj.row==-2?1:2);
else 
this.style.width=""+(t2.offsetWidth-an9)+"px";
if (this.offsetHeight<t2.offsetHeight)
an4+=(t2.offsetHeight-this.offsetHeight)/(this.msgObj.col==-2?1:2);
if (this.msgObj.col==-2){
var ao0=fpObj.GetColFooter(t2);
if (ao0)an4-=ao0.offsetHeight;
var e8=fpObj.GetCommandBar(t2);
if (e8)an4-=e8.offsetHeight;
an4-=an9;
}
this.style.top=""+an4+"px";
this.style.left=""+an5+"px";
}
this.IsVisible=true;
};
am7.Hide=function (){
this.style.visibility="hidden";
this.style.display="none";
this.IsVisible=false;
if (this.msgObj.timer){
clearTimeout(this.msgObj.timer);
this.msgObj.timer=null;
}
this.innerHTML="";
};
return am6;
}
this.GetLocation=function (ele){
if ((ele.window&&ele.window===ele)||ele.nodeType===9)return {x:0,y:0};
var ao1=0;
var ao2=0;
var ao3=null;
var ao4=null;
var ao5=null;
for (var i8=ele;i8;ao3=i8,ao4=ao5,i8=i8.offsetParent){
var al9=i8.tagName;
ao5=this.GetCurrentStyle2(i8);
if ((i8.offsetLeft||i8.offsetTop)&&
!((al9==="BODY")&&
(!ao4||ao4.position!=="absolute"))){
ao1+=i8.offsetLeft;
ao2+=i8.offsetTop;
}
if (ao3!==null&&ao5){
if ((al9!=="TABLE")&&(al9!=="TD")&&(al9!=="HTML")){
ao1+=parseInt(ao5.borderLeftWidth)||0;
ao2+=parseInt(ao5.borderTopWidth)||0;
}
if (al9==="TABLE"&&
(ao5.position==="relative"||ao5.position==="absolute")){
ao1+=parseInt(ao5.marginLeft)||0;
ao2+=parseInt(ao5.marginTop)||0;
}
}
}
ao5=this.GetCurrentStyle2(ele);
var ao6=ao5?ao5.position:null;
if (!ao6||(ao6!=="absolute")){
for (var i8=ele.parentNode;i8;i8=i8.parentNode){
al9=i8.tagName;
if ((al9!=="BODY")&&(al9!=="HTML")&&(i8.scrollLeft||i8.scrollTop)){
ao1-=(i8.scrollLeft||0);
ao2-=(i8.scrollTop||0);
ao5=this.GetCurrentStyle2(i8);
if (ao5){
ao1+=parseInt(ao5.borderLeftWidth)||0;
ao2+=parseInt(ao5.borderTopWidth)||0;
}
}
}
}
return {x:ao1,y:ao2};
}
var ao7=["borderTopWidth","borderRightWidth","borderBottomWidth","borderLeftWidth"];
var ao8=["borderTopStyle","borderRightStyle","borderBottomStyle","borderLeftStyle"];
var ao9;
this.GetBorderWidth=function (ele,side){
if (!this.GetBorderVisible(ele,side))return 0;
var n7=this.GetCurrentStyle(ele,ao7[side]);
return this.ParseBorderWidth(n7);
}
this.GetBorderVisible=function (ele,side){
return this.GetCurrentStyle(ele,ao8[side])!="none";
}
this.GetWindow=function (ele){
var aj9=ele.ownerDocument||ele.document||ele;
return aj9.defaultView||aj9.parentWindow;
}
this.GetCurrentStyle2=function (ele){
if (ele.nodeType===3)return null;
var j3=this.GetWindow(ele);
if (ele.documentElement)ele=ele.documentElement;
var ap0=(j3&&(ele!==j3))?j3.getComputedStyle(ele,null):ele.style;
return ap0;
}
this.GetCurrentStyle=function (ele,attribute,defaultValue){
var ap1=null;
if (ele){
if (ele.currentStyle){
ap1=ele.currentStyle[attribute];
}
else if (document.defaultView&&document.defaultView.getComputedStyle){
var ap2=document.defaultView.getComputedStyle(ele,null);
if (ap2){
ap1=ap2[attribute];
}
}
if (!ap1&&ele.style.getPropertyValue){
ap1=ele.style.getPropertyValue(attribute);
}
else if (!ap1&&ele.style.getAttribute){
ap1=ele.style.getAttribute(attribute);
}
}
if (!ap1||ap1==""||typeof(ap1)==='undefined'){
if (typeof(defaultValue)!='undefined'){
ap1=defaultValue;
}
else {
ap1=null;
}
}
return ap1;
}
this.ParseBorderWidth=function (n7){
if (!ao9){
var ap3={};
var ap4=document.createElement('div');
ap4.style.visibility='hidden';
ap4.style.position='absolute';
ap4.style.fontSize='1px';
document.body.appendChild(ap4)
var ap5=document.createElement('div');
ap5.style.height='0px';
ap5.style.overflow='hidden';
ap4.appendChild(ap5);
var ap6=ap4.offsetHeight;
ap5.style.borderTop='solid black';
ap5.style.borderTopWidth='thin';
ap3['thin']=ap4.offsetHeight-ap6;
ap5.style.borderTopWidth='medium';
ap3['medium']=ap4.offsetHeight-ap6;
ap5.style.borderTopWidth='thick';
ap3['thick']=ap4.offsetHeight-ap6;
ap4.removeChild(ap5);
document.body.removeChild(ap4);
ao9=ap3;
}
if (n7){
switch (n7){
case 'thin':
case 'medium':
case 'thick':
return ao9[n7];
case 'inherit':
return 0;
}
var ap7=this.ParseUnit(n7);
if (ap7.type!='px')
throw new Error();
return ap7.size;
}
return 0;
}
this.ParseUnit=function (n7){
if (!n7)
throw new Error();
n7=this.Trim(n7).toLowerCase();
var ad9=n7.length;
var t2=-1;
for (var f1=0;f1<ad9;f1++){
var ab8=n7.substr(f1,1);
if ((ab8<'0'||ab8>'9')&&ab8!='-'&&ab8!='.'&&ab8!=',')
break ;
t2=f1;
}
if (t2==-1)
throw new Error();
var am0;
var ap8;
if (t2<(ad9-1))
am0=this.Trim(n7.substring(t2+1));
else 
am0='px';
ap8=parseFloat(n7.substr(0,t2+1));
if (am0=='px'){
ap8=Math.floor(ap8);
}
return {size:ap8,type:am0};
}
this.GetViewPortByRowCol=function (e4,i9,n9){
var n5=this.GetViewport0(e4);
var f4=this.GetViewport1(e4);
var y0=this.GetViewport2(e4);
var n7=this.GetViewport(e4);
var h4=this.GetCellByRowCol(e4,i9,n9);
if (n7!=null&&this.IsChild(h4,n7))
return n7;
else if (y0!=null&&this.IsChild(h4,y0))
return y0;
else if (f4!=null&&this.IsChild(h4,f4))
return f4;
else if (n5!=null&&this.IsChild(h4,n5))
return n5;
return ;
}
this.GetMsgPos=function (e4,i9,n9){
if (i9<0||n9<0){
return {left:0,top:0};
}
else {
var n5=this.GetViewport0(e4);
var f4=this.GetViewport1(e4);
var y0=this.GetViewport2(e4);
var n7=this.GetViewport(e4);
var ap9=this.GetGroupBar(e4);
var m7=document.getElementById(e4.id+"_titleBar");
var h4=this.GetCellByRowCol(e4,i9,n9);
var g0=h4.offsetTop+h4.clientHeight;
var ad9=h4.offsetLeft;
if ((n5!=null||f4!=null)&&(this.IsChild(h4,y0)||this.IsChild(h4,n7))){
if (n5!=null)
g0+=n5.offsetHeight;
else 
g0+=f4.offsetHeight;
}
if ((n5!=null||y0!=null)&&(this.IsChild(h4,f4)||this.IsChild(h4,n7))){
if (n5!=null)
ad9+=n5.offsetWidth;
else 
ad9+=y0.offsetWidth;
}
if (n7!=null&&(n5||f4||y0)){
if (m7)g0+=m7.offsetHeight;
if (ap9)g0+=ap9.offsetHeight;
if (this.GetColHeader(e4))g0+=this.GetColHeader(e4).offsetHeight;
if (this.GetRowHeader(e4))ad9+=this.GetRowHeader(e4).offsetWidth;
}
if (n7!=null&&this.IsChild(h4,n7)){
if (f4||y0)
g0-=n7.parentNode.scrollTop;
if (f4||y0)
ad9-=n7.parentNode.scrollLeft;
}
if (y0!=null&&this.IsChild(h4,y0)){
g0-=y0.parentNode.scrollTop;
}
if (f4!=null&&this.IsChild(h4,f4)){
ad9-=f4.parentNode.scrollLeft;
}
var k0=h4.clientHeight;
var j3=h4.clientWidth;
return {left:ad9,top:g0,a4:k0,a5:j3};
}
}
this.SyncMsgs=function (e4){
if (!e4.msgList)return ;
for (f1 in e4.msgList){
if (e4.msgList[f1].constructor==Array){
for (i1 in e4.msgList[f1]){
if (e4.msgList[f1][i1]&&e4.msgList[f1][i1].msgBox&&e4.msgList[f1][i1].msgBox.IsVisible){
e4.msgList[f1][i1].msgBox.Show(e4,this);
}
}
}
}
}
this.GetCellInfo=function (e4,h1,h3,z0){
var f6=this.GetData(e4);
if (f6==null)return null;
var f7=f6.getElementsByTagName("root")[0];
if (f7==null)return null;
var o4=f7.getElementsByTagName("state")[0];
if (o4==null)return null;
var aq0=o4.getElementsByTagName("cellinfo")[0];
if (aq0==null)return null;
var g0=aq0.firstChild;
while (g0!=null){
if ((g0.getAttribute("r")==""+h1)&&(g0.getAttribute("c")==""+h3)&&(g0.getAttribute("pos")==""+z0))return g0;
g0=g0.nextSibling;
}
return null;
}
this.AddCellInfo=function (e4,h1,h3,z0){
var o1=this.GetCellInfo(e4,h1,h3,parseInt(z0));
if (o1!=null)return o1;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
if (f7==null)return null;
var o4=f7.getElementsByTagName("state")[0];
if (o4==null)return null;
var aq0=o4.getElementsByTagName("cellinfo")[0];
if (aq0==null)return null;
if (document.all!=null){
o1=f6.createNode("element","c","");
}else {
o1=document.createElement("c");
o1.style.display="none";
}
o1.setAttribute("r",h1);
o1.setAttribute("c",h3);
o1.setAttribute("pos",z0);
aq0.appendChild(o1);
return o1;
}
this.setCellAttribute=function (e4,h4,attname,x4,noEvent,recalc){
if (h4==null)return ;
var h1=this.GetRowKeyFromCell(e4,h4);
var h3=e4.getAttribute("LayoutMode")?this.GetColKeyFromCell2(e4,h4):this.GetColKeyFromCell(e4,h4);
if (typeof(h1)=="undefined")return ;
var z0=-1;
if (this.IsChild(h4,this.GetCorner(e4)))
z0=0;
else if (this.IsChild(h4,this.GetRowHeader(e4))||this.IsChild(h4,this.GetFrozColHeader(e4)))
z0=1;
else if (this.IsChild(h4,this.GetColHeader(e4))||this.IsChild(h4,this.GetFrozColHeader(e4)))
z0=2;
else if (this.IsChild(h4,this.GetViewport(e4))||this.IsChild(h4,this.GetViewport0(e4))||this.IsChild(h4,this.GetViewport1(e4))||this.IsChild(h4,this.GetViewport2(e4)))
z0=3;
var s8=this.AddCellInfo(e4,h1,h3,z0);
s8.setAttribute(attname,x4);
if (!noEvent){
var g3=this.CreateEvent("DataChanged");
g3.cell=h4;
g3.cellValue=x4;
g3.row=h1;
g3.col=h3;
this.FireEvent(e4,g3);
}
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.updateCellLocked=function (h4,locked){
if (h4==null)return ;
var g0=h4.getAttribute("FpCellType")=="readonly";
if (g0==locked)return ;
var h3=h4.firstChild;
while (h3!=null){
if (typeof(h3.disabled)!="undefined")h3.disabled=locked;
h3=h3.nextSibling;
}
}
this.Cells=function (e4,h1,h3)
{
var aq1=this.GetCellByRowCol(e4,h1,h3);
if (aq1){
aq1.GetValue=function (){
return the_fpSpread.GetValue(e4,h1,h3);
}
aq1.SetValue=function (value){
if (typeof(value)=="undefined")return ;
if (this.parentNode.getAttribute("previewRow")!=null)return ;
the_fpSpread.SetValue(e4,h1,h3,value);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetBackColor=function (){
if (this.getAttribute("bgColorBak")!=null)
return this.getAttribute("bgColorBak");
return document.defaultView.getComputedStyle(this,"").getPropertyValue("background-color");
}
aq1.SetBackColor=function (value){
if (typeof(value)=="undefined")return ;
this.bgColor=value;
this.setAttribute("bgColorBak",value);
this.style.backgroundColor=value;
the_fpSpread.setCellAttribute(e4,this,"bc",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetForeColor=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("color");
}
aq1.SetForeColor=function (value){
if (typeof(value)=="undefined")return ;
this.style.color=value;
the_fpSpread.setCellAttribute(e4,this,"fc",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetTabStop=function (){
return this.getAttribute("TabStop")!="false";
}
aq1.SetTabStop=function (value){
if (typeof(value)=="undefined")return ;
var aq2=new String(value);
if (aq2.toLocaleLowerCase()=="false"){
this.setAttribute("TabStop","false");
the_fpSpread.setCellAttribute(e4,this,"ts","false");
the_fpSpread.SaveClientEditedDataRealTime();
}else {
this.removeAttribute("TabStop");
}
}
aq1.GetCellType=function (){
var aq3=the_fpSpread.GetCellType2(this);
if (aq3=="text"||aq3=="readonly")
{
aq3=this.getAttribute("CellType2");
}
if (aq3==null)
aq3="GeneralCellType";
return aq3;
}
aq1.GetHAlign=function (){
var aq4=document.defaultView.getComputedStyle(this,"").getPropertyValue("text-Align");
if (aq4==""||aq4=="undefined"||aq4==null){
aq4=this.style.textAlign;
}
if (aq4==""||aq4=="undefined"||aq4==null)
aq4=this.getAttribute("align");
if (aq4=="start")aq4="left";
if (aq4!=null&&aq4.indexOf("-moz")!=-1)aq4=aq4.replace("-moz-","");
return aq4;
}
aq1.SetHAlign=function (value){
if (typeof(value)=="undefined")return ;
this.style.textAlign=typeof(value)=="string"?value:value.Name;
the_fpSpread.setCellAttribute(e4,this,"ha",typeof(value)=="string"?value:value.Name);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetVAlign=function (){
var aq5=document.defaultView.getComputedStyle(this,"").getPropertyValue("vertical-Align");
if (aq5==""||aq5=="undefined"||aq5==null)
aq5=this.style.verticalAlign;
if (aq5==""||aq5=="undefined"||aq5==null)
aq5=this.getAttribute("valign");
return aq5;
}
aq1.SetVAlign=function (value){
if (typeof(value)=="undefined")return ;
this.style.verticalAlign=typeof(value)=="string"?value:value.Name;
the_fpSpread.setCellAttribute(e4,this,"va",typeof(value)=="string"?value:value.Name);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetLocked=function (){
if (aq1.GetCellType()=="ButtonCellType"||aq1.GetCellType()=="TagCloudCellType"||aq1.GetCellType()=="HyperLinkCellType")
return aq1.getAttribute("Locked")=="1";
return the_fpSpread.GetCellType(this)=="readonly";
}
aq1.GetFont_Name=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-family");
}
aq1.SetFont_Name=function (value){
if (typeof(value)=="undefined")return ;
this.style.fontFamily=value;
the_fpSpread.setCellAttribute(e4,this,"fn",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetFont_Size=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-size");
}
aq1.SetFont_Size=function (value){
if (typeof(value)=="undefined")return ;
if (typeof(value)=="number")value+="px";
this.style.fontSize=value;
the_fpSpread.setCellAttribute(e4,this,"fs",value);
the_fpSpread.SizeSpread(e4);
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetFont_Bold=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-weight")=="bold"?true:false;
}
aq1.SetFont_Bold=function (value){
if (typeof(value)=="undefined")return ;
this.style.fontWeight=value==true?"bold":"normal";
the_fpSpread.setCellAttribute(e4,this,"fb",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetFont_Italic=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-style")=="italic"?true:false;
}
aq1.SetFont_Italic=function (value){
if (typeof(value)=="undefined")return ;
this.style.fontStyle=value==true?"italic":"normal";
the_fpSpread.setCellAttribute(e4,this,"fi",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetFont_Overline=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0?true:false;
}
aq1.SetFont_Overline=function (value){
if (value){
var aq6=new String("overline");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
aq6+=" line-through"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
aq6+=" underline"
this.style.textDecoration=aq6;
}
else {
var aq6=new String("");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
aq6+=" line-through"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
aq6+=" underline"
if (aq6=="")aq6="none";
this.style.textDecoration=aq6;
}
the_fpSpread.setCellAttribute(e4,this,"fo",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetFont_Strikeout=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0?true:false;
}
aq1.SetFont_Strikeout=function (value){
if (value){
var aq6=new String("line-through");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
aq6+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
aq6+=" underline"
this.style.textDecoration=aq6;
}
else {
var aq6=new String("");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
aq6+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
aq6+=" underline"
if (aq6=="")aq6="none";
this.style.textDecoration=aq6;
}
the_fpSpread.setCellAttribute(e4,this,"fk",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
aq1.GetFont_Underline=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0?true:false;
}
aq1.SetFont_Underline=function (value){
if (value){
var aq6=new String("underline");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
aq6+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
aq6+=" line-through"
this.style.textDecoration=aq6;
}
else {
var aq6=new String("");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
aq6+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
aq6+=" line-through"
if (aq6=="")aq6="none";
this.style.textDecoration=aq6;
}
the_fpSpread.setCellAttribute(e4,this,"fu",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
return aq1;
}
return null;
}
this.getDomRow=function (e4,h1){
var n6=this.GetRowCount(e4);
if (n6==0)return null;
var h4=this.GetCellByRowCol(e4,h1,0);
if (h4){
var f0=h4.parentNode.rowIndex;
if (f0>=0){
var i9=h4.parentNode.parentNode.rows[f0];
if (this.GetSizable(e4,i9))
return i9;
}
return null;
}
}
this.setRowInfo_RowAttribute=function (e4,h1,attname,x4,recalc){
h1=parseInt(h1);
if (h1<0)return ;
var aq7=this.AddRowInfo(e4,h1);
aq7.setAttribute(attname,x4);
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.Rows=function (e4,h1)
{
var aq8=this.getDomRow(e4,h1);
if (aq8){
aq8.GetHeight=function (){
return the_fpSpread.GetRowHeightInternal(e4,h1);
}
aq8.SetHeight=function (value){
if (typeof(value)=="undefined")return ;
the_fpSpread.SetRowHeight2(e4,h1,parseInt(value));
the_fpSpread.SaveClientEditedDataRealTime();
}
return aq8;
}
return null;
}
this.setColInfo_ColumnAttribute=function (e4,h3,attname,x4,recalc){
h3=parseInt(h3);
if (h3<0)return ;
var aq9=this.AddColInfo(e4,h3);
aq9.setAttribute(attname,x4);
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.Columns=function (e4,h3)
{
var ar0={a2:this.GetColByKey(e4,parseInt(h3))};
if (ar0){
ar0.GetWidth=function (){
return the_fpSpread.GetColWidthFromCol(e4,h3);
}
ar0.SetWidth=function (value){
if (typeof(value)=="undefined")return ;
the_fpSpread.SetColWidth(e4,h3,value);
the_fpSpread.SaveClientEditedDataRealTime();
}
return ar0;
}
return null;
}
this.GetTitleBar=function (e4){
try {
if (document.getElementById(e4.id+"_title")==null)return null;
var ar1=document.getElementById(e4.id+"_titleBar");
if (ar1!=null)ar1=document.getElementById(e4.id+"_title");
return ar1;
}
catch (ex){
return null;
}
}
this.CheckTitleInfo=function (e4){
var f6=this.GetData(e4);
if (f6==null)return null;
var f7=f6.getElementsByTagName("root")[0];
if (f7==null)return null;
var ar2=f7.getElementsByTagName("titleinfo")[0];
if (ar2==null)return null;
return ar2;
}
this.AddTitleInfo=function (e4){
var o1=this.CheckTitleInfo(e4);
if (o1!=null)return o1;
var f6=this.GetData(e4);
var f7=f6.getElementsByTagName("root")[0];
if (f7==null)return null;
if (document.all!=null){
o1=f6.createNode("element","titleinfo","");
}else {
o1=document.createElement("titleinfo");
o1.style.display="none";
}
f7.appendChild(o1);
return o1;
}
this.setTitleInfo_Attribute=function (e4,attname,x4,recalc){
var ar3=this.AddTitleInfo(e4);
ar3.setAttribute(attname,x4);
var f9=this.GetCmdBtn(e4,"Update");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
f9=this.GetCmdBtn(e4,"Cancel");
if (f9!=null&&f9.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f9,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.GetTitleInfo=function (e4)
{
var ar4=this.GetTitleBar(e4);
if (ar4){
ar4.GetHeight=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("height");
}
ar4.SetHeight=function (value){
this.style.height=parseInt(value)+"px";
the_fpSpread.setTitleInfo_Attribute(e4,"ht",value);
var e9=the_fpSpread.GetTopSpread(e4);
the_fpSpread.SizeAll(e9);
the_fpSpread.Refresh(e9);
the_fpSpread.SaveClientEditedDataRealTime();
}
ar4.GetVisible=function (){
return (document.defaultView.getComputedStyle(this,"").getPropertyValue("display")=="none")?false:true;
return document.defaultView.getComputedStyle(this,"").getPropertyValue("visibility");
}
ar4.SetVisible=function (value){
this.style.display=value?"":"none";
this.style.visibility=value?"visible":"hidden";
the_fpSpread.setTitleInfo_Attribute(e4,"vs",new String(value).toLocaleLowerCase());
var e9=the_fpSpread.GetTopSpread(e4);
the_fpSpread.SizeAll(e9);
the_fpSpread.Refresh(e9);
the_fpSpread.SaveClientEditedDataRealTime();
}
ar4.GetValue=function (){
return this.textContent;
}
ar4.SetValue=function (value){
this.textContent=""+value;
the_fpSpread.setTitleInfo_Attribute(e4,"tx",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
return ar4;
}
return null;
}
this.SaveClientEditedDataRealTime=function (){
var ar5=this.GetPageActiveSpread();
if (ar5!=null){
this.SaveData(ar5);
ar5.e2=false;
}
ar5=this.GetPageActiveSheetView();
if (ar5!=null){
this.SaveData(ar5);
ar5.e2=false;
}
}
var ar6="";
this.ShowScrollingContent=function (e4,hs){
var s9="";
var p8=this.GetTopSpread(e4);
var ar7=p8.getAttribute("scrollContentColumns");
var ar8=p8.getAttribute("scrollContentMaxHeight");
var ar9=p8.getAttribute("scrollContentTime");
var i6=this.GetViewport(p8);
var as0=this.GetColGroup(i6);
var n7=this.GetParent(i6);
var as1=0;
if (hs){
var as2=n7.scrollLeft;
var c6=this.GetColHeader(p8);
var u6=0;
for (;u6<as0.childNodes.length;u6++){
var h3=as0.childNodes[u6];
as1+=h3.offsetWidth;
if (as1>as2)break ;
}
var as3=this.GetViewport2(p8);
if (as3)u6+=this.GetColGroup(as3).childNodes.length;
if (c6){
var u0=c6.rows.length-1;
if (e4.getAttribute("LayoutMode")==null)
u0=c6.getAttribute("ColTextIndex")?c6.getAttribute("ColTextIndex"):c6.rows.length-1;
var as4=this.GetHeaderCellFromRowCol(p8,u0,u6,true);
if (as4){
if (as4.getAttribute("FpCellType")=="ExtenderCellType"&&as4.getElementsByTagName("DIV").length>0){
var z9=this.GetEditor(as4);
var aa0=this.GetFunction("ExtenderCellType_getEditorValue");
if (z9!==null&&aa0!==null){
s9="&nbsp;Column:&nbsp;"+aa0(z9)+"&nbsp;";
}
}
else 
s9="&nbsp;Column:&nbsp;"+as4.innerHTML+"&nbsp;";
}
}
if (s9.length<=0)s9="&nbsp;Column:&nbsp;"+(u6+1)+"&nbsp;"
}
else {
var o4=n7.scrollTop;
var c5=this.GetRowHeader(p8);
var u0=0;
var as5=0;
var as6=2;
for (var ab1=0;ab1<i6.rows.length;ab1++){
var h1=i6.rows[ab1];
as1+=h1.offsetHeight;
if (as1>o4){
if (h1.getAttribute("fpkey")==null&&!h1.getAttribute("previewrow"))
u0--;
else 
as5=h1.offsetHeight;
break ;
}
if (h1.getAttribute("fpkey")!=null||h1.getAttribute("previewrow")){
u0++;
as5=h1.offsetHeight;
}
}
var as3=this.GetViewport1(p8);
if (as3)u0+=as3.rows.length;
if (e4.getAttribute("LayoutMode")==null&&ar7!=null&&ar7.length>0){
as5=as5>ar8?ar8:as5;
var as7=ar7.split(",");
var as8=false;
for (var f1=0;f1<as7.length;f1++){
var h3=parseInt(as7[f1]);
if (h3==null||h3>=this.GetColCount(e4))continue ;
var h4=p8.GetCellByRowCol(u0,h3);
if (!h4||h4.getAttribute("col")!=null&&h4.getAttribute("col")!=h3)continue ;
var as9=(h4.getAttribute("group")==1);
var af5=(h4.parentNode.getAttribute("previewrow")!=null);
var g3=(h4.getAttribute("RowEditTemplate")!=null);
var j9=this.IsXHTML(e4);
if (!j9&&ar6==""){
this.GetScrollingContentStyle(e4);
if (am9!=null){
if (am9.fontFamily!=null&&am9.fontFamily!="")ar6+="fontFamily:"+am9.fontFamily+";";
if (am9.fontSize!=null&&am9.fontSize!="")ar6+="fontSize:"+am9.fontSize+";";
if (am9.fontStyle!=null&&am9.fontStyle!="")ar6+="fontStyle:"+am9.fontStyle+";";
if (am9.fontVariant!=null&&am9.fontVariant!="")ar6+="fontVariant:"+am9.fontVariant+";";
if (am9.fontWeight!=null&&am9.fontWeight!="")ar6+="fontWeight:"+am9.fontWeight+";";
if (am9.backgroundColor!=null&&am9.backgroundColor!="")ar6+="backgroundColor:"+am9.backgroundColor+";";
if (am9.color!=null&&am9.color!="")ar6+="color:"+am9.color;
}
}
if (!as8){
s9+="<div style='overflow:hidden;height:"+as5+"px;ScrollingContentWidth'><table cellPadding='0' cellSpacing='0' style='height:"+as5+"px;"+(as9?"":"table-layout:auto;")+ar6+"'><tr>";
}
s9+="<td style='width:"+(as9?0:h4.offsetWidth)+"px;'>";
as6+=h4.offsetWidth;
if (as9)
s9+="&nbsp;<i>GroupBar:</i>&nbsp;"+h4.textContent+"&nbsp;";
else if (af5)
s9+="&nbsp;<i>PreviewRow:</i>&nbsp;"+h4.textContent+"&nbsp;";
else if (g3){
var at0=this.parseCell(e4,h4);
s9+="&nbsp;<i>RowEditTemplate:</i>&nbsp;"+at0+"&nbsp;"
}
else {
if (h4.getAttribute("fpcelltype"))this.UpdateCellTypeDOM(h4);
if (h4.getAttribute("fpcelltype")=="MultiColumnComboBoxCellType"&&h4.childNodes[0]&&h4.childNodes[0].childNodes.length>0&&h4.childNodes[0].getAttribute("MccbId"))
s9+=p8.GetValue(u0,h3);
else if (h4.getAttribute("fpcelltype")=="RadioButtonListCellType"||h4.getAttribute("fpcelltype")=="ExtenderCellType"||h4.getAttribute("fpeditorid")!=null){
var at1=this.parseCell(e4,h4);
s9+=at1;
}
else 
s9+=h4.innerHTML;
}
s9+="</td>";
as8=true;
if (as9||af5||g3)break ;
}
if (as8){
s9=s9.replace("ScrollingContentWidth"," width:"+as6+"px;");
s9+="</tr></table></div>";
}
}
if (s9.length<=0&&c5){
var u6=this.GetColGroup(c5).childNodes.length-1;
if (e4.getAttribute("LayoutMode")==null)
u6=c5.getAttribute("RowTextIndex")?parseInt(c5.getAttribute("RowTextIndex")):this.GetColGroup(c5).childNodes.length-1;
var i9=this.GetDisplayIndex(e4,u0);
var as4=this.GetHeaderCellFromRowCol(e4,i9,u6,false);
if (as4)s9="&nbsp;Row:&nbsp;"+as4.textContent+"&nbsp;";
}
if (s9.length<=0){
var n6=(e4.getAttribute("layoutrowcount")!=null)?parseInt(e4.getAttribute("layoutrowcount")):1;
s9="&nbsp;Row:&nbsp;"+(parseInt(u0/n6)+1)+"&nbsp;";
}
}
this.ShowMessageInner(p8,s9,(hs?-1:-2),(hs?-2:-1),ar9);
}
this.parseCell=function (e4,h4){
var s9=h4.innerHTML;
var p8=this.GetTopSpread(e4);
var at2=p8.id;
if (s9.length>0){
s9=s9.replace(new RegExp("=\""+at2,"g"),"=\""+at2+"src");
s9=s9.replace(new RegExp("name="+at2,"g"),"name="+at2+"src");
}
return s9;
}
this.UpdateCellTypeDOM=function (h4){
for (var f1=0;f1<h4.childNodes.length;f1++){
if (h4.childNodes[f1].tagName&&(h4.childNodes[f1].tagName=="INPUT"||h4.childNodes[f1].tagName=="SELECT"))
this.UpdateDOM(h4.childNodes[f1]);
if (h4.childNodes[f1].childNodes&&h4.childNodes[f1].childNodes.length>0)
this.UpdateCellTypeDOM(h4.childNodes[f1]);
}
}
this.UpdateDOM=function (inputField){
if (typeof(inputField)=="string"){
inputField=document.getElementById(inputField);
}
if (inputField.type=="select-one"){
for (var f1=0;f1<inputField.options.length;f1++){
if (f1==inputField.selectedIndex){
inputField.options[inputField.selectedIndex].setAttribute("selected","selected");
}
}
}
else if (inputField.type=="text"){
inputField.setAttribute("value",inputField.value);
}
else if (inputField.type=="textarea"){
inputField.setAttribute("value",inputField.value);
}
else if ((inputField.type=="checkbox")||(inputField.type=="radio")){
if (inputField.checked){
inputField.setAttribute("checked","checked");
}else {
inputField.removeAttribute("checked");
}
}
}
this.GetScrollingContentStyle=function (e4){
if (am9!=null)return ;
var f0=document.styleSheets.length;
for (var f1=0;f1<f0;f1++){
var at3=document.styleSheets[f1];
for (var i1=0;i1<at3.cssRules.length;i1++){
var at4=at3.cssRules[i1];
if (at4.selectorText=="."+e4.id+"scrollContentStyle"){
am9=at4.style;
break ;
}
}
if (am9!=null)break ;
}
}
}
function ComboBoxCellType_setValue(x8,x4,e4){
var h4=the_fpSpread.GetCell(x8);
if (h4==null)return ;
var at5=h4.getElementsByTagName("SELECT");
if (at5!=null&&at5.length>0){
at5[0].value=x4;
return 
}
var w0=the_fpSpread.GetCellEditorID(e4,h4);
var a8=null;
if (w0!=null&&typeof(w0)!="undefined"){
a8=the_fpSpread.GetCellEditor(e4,w0,true);
if (a8!=null){
a8.value=x4;
if (a8.selectedIndex>=0&&a8.selectedIndex<a8.options.length)
x4=a8.options[a8.selectedIndex].text;
if (x4!=null&&x4!="")
x8.innerHTML=x4;
else 
x8.innerHTML="&nbsp;";
}
}
}
function ComboBoxCellType_getValue(x8,e4){
var v9=x8.innerHTML;
var j2=the_fpSpread.GetCell(x8);
var at5=j2.getElementsByTagName("SELECT");
if (at5!=null&&at5.length>0){
return at5[0].value;
}
var w0=the_fpSpread.GetCellEditorID(e4,j2);
var a8=null;
if (w0!=null&&typeof(w0)!="undefined"){
a8=the_fpSpread.GetCellEditor(e4,w0,true);
if (a8!=null){
var f0=a8.options.length;
for (var f1=0;f1<f0;f1++){
if (a8.options[f1].text==v9){
return a8.options[f1].value;
}
}
return null;
}
}
return v9;
}
function CheckBoxCellType_setFocus(h4){
var i4=h4.getElementsByTagName("INPUT");
if (i4!=null&&i4.length>0&&i4[0].type=="checkbox"){
i4[0].focus();
}
}
function CheckBoxCellType_getCheckBoxEditor(h4){
var i4=h4.getElementsByTagName("INPUT");
if (i4!=null&&i4.length>0&&i4[0].type=="checkbox"){
return i4[0];
}
return null;
}
function CheckBoxCellType_isValid(h4,x4){
if (x4==null)return "";
x4=the_fpSpread.Trim(x4);
if (x4=="")return "";
if (x4.toLowerCase()=="true"||x4.toLowerCase()=="false")
return "";
else 
return "invalid value";
}
function CheckBoxCellType_getValue(x8,e4){
return CheckBoxCellType_getEditorValue(x8,e4);
}
function CheckBoxCellType_getEditorValue(x8,e4){
var h4=the_fpSpread.GetCell(x8);
var i4=CheckBoxCellType_getCheckBoxEditor(h4);
if (i4!=null&&i4.checked){
return "true";
}
return "false";
}
function CheckBoxCellType_setValue(x8,x4){
var h4=the_fpSpread.GetCell(x8);
var i4=CheckBoxCellType_getCheckBoxEditor(h4);
if (i4!=null){
i4.checked=(x4!=null&&x4.toLowerCase()=="true");
return ;
}
}
function IntegerCellType_getValue(x8){
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
if (g0.innerHTML=="&nbsp;")return "";
var v9=g0.innerHTML;
x8=the_fpSpread.GetCell(x8);
if (x8.getAttribute("FpRef")!=null)x8=document.getElementById(x8.getAttribute("FpRef"));
var at6=x8.getAttribute("groupchar");
if (at6==null)at6=",";
var w5=v9.length;
while (true){
v9=v9.replace(at6,"");
if (v9.length==w5)break ;
w5=v9.length;
}
if (v9.charAt(0)=='('&&v9.charAt(v9.length-1)==')'){
var at7=x8.getAttribute("negsign");
if (at7==null)at7="-";
v9=at7+v9.substring(1,v9.length-1);
}
v9=the_fpSpread.ReplaceAll(v9,"&nbsp;"," ");
return v9;
}
function IntegerCellType_isValid(h4,x4){
if (x4==null||x4.length==0)return "";
x4=x4.replace(" ","");
if (x4.length==0)return "";
var as1=h4;
var at8=h4.getAttribute("FpRef");
if (at8!=null)as1=document.getElementById(at8);
var at7=as1.getAttribute("negsign");
var z0=as1.getAttribute("possign");
if (at7!=null)x4=x4.replace(at7,"-");
if (z0!=null)x4=x4.replace(z0,"+");
if (x4.charAt(x4.length-1)=="-")x4="-"+x4.substring(0,x4.length-1);
var w7=new RegExp("^\\s*[-\\+]?\\d+\\s*$");
var p4=(x4.match(w7)!=null);
if (p4)p4=!isNaN(x4);
if (p4){
var x1=as1.getAttribute("MinimumValue");
var j1=as1.getAttribute("MaximumValue");
var x0=parseInt(x4);
if (x1!=null){
x1=parseInt(x1);
p4=(!isNaN(x1)&&x0>=x1);
}
if (p4&&j1!=null){
j1=parseInt(j1);
p4=(!isNaN(j1)&&x0<=j1);
}
}
if (!p4){
if (as1.getAttribute("error")!=null)
return as1.getAttribute("error");
else 
return "Integer";
}
return "";
}
function DoubleCellType_isValid(h4,x4){
if (x4==null||x4.length==0)return "";
var as1=h4;
if (h4.getAttribute("FpRef")!=null)as1=document.getElementById(h4.getAttribute("FpRef"));
var at9=as1.getAttribute("decimalchar");
if (at9==null)at9=".";
var at6=as1.getAttribute("groupchar");
if (at6==null)at6=",";
x4=the_fpSpread.Trim(x4);
var p4=true;
p4=(x4.length==0||x4.charAt(0)!=at6);
if (p4){
var f0=x4.indexOf(at9);
if (f0>=0){
f0=x4.indexOf(at6,f0);
p4=(f0<0);
}
}
if (p4){
var w5=x4.length;
while (true){
x4=x4.replace(at6,"");
if (x4.length==w5)break ;
w5=x4.length;
}
}
if (x4.length==0){
p4=false;
}else if (p4){
var at7=as1.getAttribute("negsign");
var z0=as1.getAttribute("possign");
var x1=as1.getAttribute("MinimumValue");
var j1=as1.getAttribute("MaximumValue");
p4=the_fpSpread.IsDouble(x4,at9,at7,z0,x1,j1);
}
if (!p4){
if (as1.getAttribute("error")!=null)
return as1.getAttribute("error");
else 
return "Double";
}
return "";
}
function DoubleCellType_getValue(x8){
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
if (g0.innerHTML=="&nbsp;")return "";
var v9=g0.innerHTML;
x8=the_fpSpread.GetCell(x8);
if (x8.getAttribute("FpRef")!=null)x8=document.getElementById(x8.getAttribute("FpRef"));
var at6=x8.getAttribute("groupchar");
if (at6==null)at6=",";
var w5=v9.length;
while (true){
v9=v9.replace(at6,"");
if (v9.length==w5)break ;
w5=v9.length;
}
if (v9.charAt(0)=='('&&v9.charAt(v9.length-1)==')'){
var at7=x8.getAttribute("negsign");
if (at7==null)at7="-";
v9=at7+v9.substring(1,v9.length-1);
}
v9=the_fpSpread.ReplaceAll(v9,"&nbsp;"," ");
return v9;
}
function CurrencyCellType_isValid(h4,x4){
if (x4!=null&&x4.length>0){
var as1=h4;
if (h4.getAttribute("FpRef")!=null)as1=document.getElementById(h4.getAttribute("FpRef"));
var w4=as1.getAttribute("currencychar");
if (w4==null)w4="$";
x4=x4.replace(w4,"");
var at6=as1.getAttribute("groupchar");
if (at6==null)at6=",";
var at9=as1.getAttribute("decimalchar");
if (at9==null)at9=".";
x4=the_fpSpread.Trim(x4);
var p4=true;
p4=(x4.length==0||x4.charAt(0)!=at6);
if (p4){
var f0=x4.indexOf(at9);
if (f0>=0){
f0=x4.indexOf(at6,f0);
p4=(f0<0);
}
}
if (p4){
var w5=x4.length;
while (true){
x4=x4.replace(at6,"");
if (x4.length==w5)break ;
w5=x4.length;
}
}
var p4=true;
if (x4.length==0){
p4=false;
}else if (p4){
var at7=as1.getAttribute("negsign");
var z0=as1.getAttribute("possign");
var x1=as1.getAttribute("MinimumValue");
var j1=as1.getAttribute("MaximumValue");
p4=the_fpSpread.IsDouble(x4,at9,at7,z0,x1,j1);
}
if (!p4){
if (as1.getAttribute("error")!=null)
return as1.getAttribute("error");
else 
return "Currency ("+w4+"100"+at9+"10) ";
}
}
return "";
}
function CurrencyCellType_getValue(x8){
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
if (g0.innerHTML=="&nbsp;")return "";
var v9=g0.innerHTML;
x8=the_fpSpread.GetCell(x8);
if (x8.getAttribute("FpRef")!=null)x8=document.getElementById(x8.getAttribute("FpRef"));
var w4=x8.getAttribute("currencychar");
if (w4!=null){
var au0=document.createElement("SPAN");
au0.innerHTML=w4;
w4=au0.innerHTML;
}
if (w4==null)w4="$";
var at6=x8.getAttribute("groupchar");
if (at6==null)at6=",";
v9=v9.replace(w4,"");
var w5=v9.length;
while (true){
v9=v9.replace(at6,"");
if (v9.length==w5)break ;
w5=v9.length;
}
var at7=x8.getAttribute("negsign");
if (at7==null)at7="-";
if (v9.charAt(0)=='('&&v9.charAt(v9.length-1)==')'){
v9=at7+v9.substring(1,v9.length-1);
}
v9=the_fpSpread.ReplaceAll(v9,"&nbsp;"," ");
return v9;
}
function RegExpCellType_isValid(h4,x4){
if (x4==null||x4=="")
return "";
var as1=h4;
if (h4.getAttribute("FpRef")!=null)as1=document.getElementById(h4.getAttribute("FpRef"));
var au1=new RegExp(as1.getAttribute("fpexpression"));
var w8=x4.match(au1);
var n7=(w8!=null&&w8.length>0&&x4==w8[0]);
if (!n7){
if (as1.getAttribute("error")!=null)
return as1.getAttribute("error");
else 
return "invalid";
}
return "";
}
function PercentCellType_getValue(x8){
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
if (g0.innerHTML=="&nbsp;")return "";
g0=g0.innerHTML;
var h4=the_fpSpread.GetCell(x8);
var as1=h4;
if (h4.getAttribute("FpRef")!=null)as1=document.getElementById(h4.getAttribute("FpRef"));
var au2=as1.getAttribute("percentchar");
if (au2==null)au2="%";
g0=g0.replace(au2,"");
var at6=as1.getAttribute("groupchar");
if (at6==null)at6=",";
var w5=g0.length;
while (true){
g0=g0.replace(at6,"");
if (g0.length==w5)break ;
w5=g0.length;
}
var at7=as1.getAttribute("negsign");
var z0=as1.getAttribute("possign");
g0=the_fpSpread.ReplaceAll(g0,"&nbsp;"," ");
var g5=g0;
if (at7!=null)
g0=g0.replace(at7,"-");
if (z0!=null)
g0=g0.replace(z0,"+");
var at9=as1.getAttribute("decimalchar");
if (at9!=null)
g0=g0.replace(at9,".");
if (!isNaN(g0))
return g5;
else 
return x8.innerHTML;
}
function PercentCellType_setValue(x8,x4){
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
x8=g0;
if (x4!=null&&x4!=""){
var as1=the_fpSpread.GetCell(x8);
if (as1.getAttribute("FpRef")!=null)as1=document.getElementById(as1.getAttribute("FpRef"));
var au2=as1.getAttribute("percentchar");
if (au2==null)au2="%";
x4=x4.replace(" ","");
x4=x4.replace(au2,"");
x8.innerHTML=x4+au2;
}else {
x8.innerHTML="";
}
}
function PercentCellType_isValid(h4,x4){
if (x4!=null){
var as1=the_fpSpread.GetCell(h4);
if (as1.getAttribute("FpRef")!=null)as1=document.getElementById(as1.getAttribute("FpRef"));
var au2=as1.getAttribute("percentchar");
if (au2==null)au2="%";
x4=x4.replace(au2,"");
var at6=as1.getAttribute("groupchar");
if (at6==null)at6=",";
var w5=x4.length;
while (true){
x4=x4.replace(at6,"");
if (x4.length==w5)break ;
w5=x4.length;
}
var au3=x4;
var at7=as1.getAttribute("negsign");
var z0=as1.getAttribute("possign");
if (at7!=null)x4=x4.replace(at7,"-");
if (z0!=null)x4=x4.replace(z0,"+");
var at9=as1.getAttribute("decimalchar");
if (at9!=null)
x4=x4.replace(at9,".");
var p4=!isNaN(x4);
if (p4){
var au4=as1.getAttribute("MinimumValue");
var au5=as1.getAttribute("MaximumValue");
if (au4!=null||au5!=null){
var x1=parseFloat(au4);
var j1=parseFloat(au5);
p4=!isNaN(x1)&&!isNaN(j1);
if (p4){
if (at9==null)at9=".";
p4=the_fpSpread.IsDouble(au3,at9,at7,z0,x1*100,j1*100);
}
}
}
if (!p4){
if (as1.getAttribute("error")!=null)
return as1.getAttribute("error");
else 
return "Percent:(ex,10"+au2+")";
}
}
return "";
}
function ListBoxCellType_getValue(x8){
var g0=x8.getElementsByTagName("TABLE");
if (g0.length>0)
{
var g9=g0[0].rows;
for (var i1=0;i1<g9.length;i1++){
var h4=g9[i1].cells[0];
if (h4.selected=="true")
{
var au6=h4;
while (au6.firstChild!=null)au6=au6.firstChild;
var as1=au6.nodeValue;
return as1;
}
}
}
return "";
}
function ListBoxCellType_setValue(x8,x4){
var g0=x8.getElementsByTagName("TABLE");
if (g0.length>0)
{
g0[0].style.width=(x8.clientWidth-6)+"px";
var g9=g0[0].rows;
for (var i1=0;i1<g9.length;i1++){
var h4=g9[i1].cells[0];
var au6=h4;
while (au6.firstChild!=null)au6=au6.firstChild;
var as1=au6.nodeValue;
if (as1==x4){
h4.selected="true";
if (g0[0].parentNode.getAttribute("selectedBackColor")!="undefined")
h4.style.backgroundColor=g0[0].parentNode.getAttribute("selectedBackColor");
if (g0[0].parentNode.getAttribute("selectedForeColor")!="undefined")
h4.style.color=g0[0].parentNode.getAttribute("selectedForeColor");
}else {
h4.style.backgroundColor="";
h4.style.color="";
h4.selected="";
h4.bgColor="";
}
}
}
}
function TextCellType_getValue(x8){
var h4=the_fpSpread.GetCell(x8,true);
if (h4!=null&&h4.getAttribute("password")!=null){
if (h4!=null&&h4.getAttribute("value")!=null)
return h4.getAttribute("value");
else 
return "";
}else {
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
if (g0.innerHTML=="&nbsp;")return "";
if (g0!=null){
if (g0.tagName=="INPUT")
g0=g0.value;
else 
g0=the_fpSpread.HTMLDecode(g0.innerHTML);
}
g0=the_fpSpread.ReplaceAll(g0,"<br>","\n");
return g0;
}
}
function TextCellType_setValue(x8,x4){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
var g0=x8;
while (g0.firstChild!=null&&g0.firstChild.nodeName!="#text")g0=g0.firstChild;
x8=g0;
if (h4.getAttribute("password")!=null){
if (x4!=null&&x4!=""){
x4=x4.replace(" ","");
x8.innerHTML="";
for (var f1=0;f1<x4.length;f1++)
x8.innerHTML+="*";
h4.setAttribute("value",x4);
}else {
x8.innerHTML="";
h4.setAttribute("value","");
}
}else {
if (x4!=null)x4=the_fpSpread.HTMLEncode(x4);
x4=the_fpSpread.ReplaceAll(x4,"\n","<br>");
x4=the_fpSpread.ReplaceAll(x4," ","&nbsp;");
x8.innerHTML=x4;
}
}
function TextCellType_setEditorValue(g0,x4){
if (x4!=null)x4=the_fpSpread.HTMLDecode(x4);
g0.value=x4;
}
function RadioButtonListCellType_getValue(x8){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
var au7=h4.getElementsByTagName("INPUT");
for (var f1=0;f1<au7.length;f1++){
if (au7[f1].tagName=="INPUT"&&au7[f1].checked){
return au7[f1].value;
}
}
return "";
}
function RadioButtonListCellType_getEditorValue(x8){
return RadioButtonListCellType_getValue(x8);
}
function RadioButtonListCellType_setValue(x8,x4){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
if (x4!=null)x4=the_fpSpread.Trim(x4);
var au7=h4.getElementsByTagName("INPUT");
for (var f1=0;f1<au7.length;f1++){
if (au7[f1].tagName=="INPUT"&&x4==the_fpSpread.Trim(au7[f1].value)){
au7[f1].checked=true;
break ;
}else {
if (au7[f1].checked)au7[f1].checked=false;
}
}
}
function RadioButtonListCellType_setFocus(x8){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
var i4=h4.getElementsByTagName("INPUT");
if (i4==null)return ;
for (var f1=0;f1<i4.length;f1++){
if (i4[f1].type=="radio"&&i4[f1].checked){
i4[f1].focus();
return ;
}
}
}
function MultiColumnComboBoxCellType_setValue(x8,x4,e4){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
var at5=h4.getElementsByTagName("DIV");
if (at5!=null&&at5.length>0){
var au8=h4.getElementsByTagName("input");
if (au8!=null&&au8.length>0)
au8[0].value=x4;
return ;
}
if (x4!=null&&x4!="")
x8.textContent=x4;
else 
x8.innerHTML="&nbsp;";
}
function MultiColumnComboBoxCellType_getValue(x8,e4){
var v9=x8.textContent;
var j2=the_fpSpread.GetCell(x8,true);
var at5=j2.getElementsByTagName("DIV");
if (at5!=null&&at5.length>0){
var au8=j2.getElementsByTagName("input");
if (au8!=null&&au8.length>0)
return au8[0].value;
return ;
}
if (!e4)return null;
var w0=the_fpSpread.GetCellEditorID(e4,j2);
var a8=null;
if (w0!=null&&typeof(w0)!="undefined"){
a8=the_fpSpread.GetCellEditor(e4,w0,true);
if (a8!=null){
var au9=a8.getAttribute("MccbId");
if (au9){
FarPoint.System.WebControl.MultiColumnComboBoxCellType.CheckInit(au9);
var ai8=eval(au9+"_Obj");
if (ai8!=null&&ai8.SetText!=null){
ai8.SetText(v9);
return v9;
}
}
}
return null;
}
return v9;
}
function MultiColumnComboBoxCellType_getEditorValue(x8,e4){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
var av0=h4.getElementsByTagName("INPUT");
if (av0!=null&&av0.length>0){
var g0=av0[0];
return g0.value;
}
return null;
}
function MultiColumnComboBoxCellType_setFocus(x8){
var h4=the_fpSpread.GetCell(x8);
var e4=the_fpSpread.GetSpread(h4);
if (h4==null)return ;
var av1=h4.getElementsByTagName("DIV");
if (av1!=null&&av1.length>0){
var au9=av1[0].getAttribute("MccbId");
if (au9){
var ai8=eval(au9+"_Obj");
if (ai8!=null&&typeof(ai8.FocusForEdit)!="undefined"){
ai8.FocusForEdit();
}
}
}
}
function MultiColumnComboBoxCellType_setEditorValue(x8,editorValue,e4){
var h4=the_fpSpread.GetCell(x8,true);
if (h4==null)return ;
var w0=the_fpSpread.GetCellEditorID(e4,h4);
var a8=null;
if (w0!=null&&typeof(w0)!="undefined"){
a8=the_fpSpread.GetCellEditor(e4,w0,true);
if (a8!=null){
var au9=a8.getAttribute("MccbId");
if (au9){
FarPoint.System.WebControl.MultiColumnComboBoxCellType.CheckInit(au9);
var ai8=eval(au9+"_Obj");
if (ai8!=null&&ai8.SetText!=null){
ai8.SetText(editorValue);
}
}
}
}
}
function TagCloudCellType_getValue(x8,e4){
var v9=x8.textContent;
if (typeof(v9)!="undefined"&&v9!=null&&v9.length>0)
{
v9=the_fpSpread.ReplaceAll(v9,"<br>","");
v9=the_fpSpread.ReplaceAll(v9,"\n","");
v9=the_fpSpread.ReplaceAll(v9,"\t","");
var t0=new RegExp("\xA0","g");
v9=v9.replace(t0,String.fromCharCode(32));
v9=the_fpSpread.HTMLDecode(v9);
}
else 
v9="";
return v9;
}
