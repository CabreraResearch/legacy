//
//
//	Copyright?2005. FarPoint Technologies.	All rights reserved.
//
var the_fpSpread = new Fpoint_FPSpread();
function FpSpread_EventHandlers(){
var e3=the_fpSpread;
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
var e5=e3.GetViewport(e4);
if (e5!=null){
e3.AttachEvent(e3.GetViewport(e4).parentNode,"DOMAttrModified",this.DoPropertyChange,true);
e3.AttachEvent(e3.GetViewport(e4).parentNode,"scroll",this.ScrollViewport);
}
var e6=e3.GetCommandBar(e4);
if (e6!=null){
e3.AttachEvent(e6,"mouseover",this.CmdbarMouseOver,false);
e3.AttachEvent(e6,"mouseout",this.CmdbarMouseOut,false);
}
}
this.DetachEvents=function (e4){
e3.DetachEvent(e4,"mousedown",this.MouseDown,false);
e3.DetachEvent(e4,"mouseup",this.MouseUp,false);
e3.DetachEvent(document,"mouseup",this.MouseUp,false);
e3.DetachEvent(e4,"mousemove",this.MouseMove,false);
e3.DetachEvent(e4,"dblclick",this.DblClick,false);
e3.DetachEvent(e4,"focus",this.Focus,false);
var e5=e3.GetViewport(e4);
if (e5!=null){
e3.DetachEvent(e3.GetViewport(e4).parentNode,"DOMAttrModified",this.DoPropertyChange,true);
e3.DetachEvent(e3.GetViewport(e4).parentNode,"scroll",this.ScrollViewport);
}
var e6=e3.GetCommandBar(e4);
if (e6!=null){
e3.DetachEvent(e6,"mouseover",this.CmdbarMouseOver,false);
e3.DetachEvent(e6,"mouseout",this.CmdbarMouseOut,false);
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
this.left=37;
this.right=39;
this.up=38;
this.down=40;
this.tab=9;
this.enter=13;
this.shift=16;
this.space=32;
this.altkey=18;
this.home=36;
this.end=35;
this.pup=33;
this.pdn=34;
this.backspace=8;
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
e4.footerSpanCells=new Array();
this.activePager=null;
this.dragSlideBar=false;
e4.allowColMove=(e4.getAttribute("colMove")=="true");
e4.allowGroup=(e4.getAttribute("allowGroup")=="true");
e4.selectedCols=[];
e4.msgList=new Array();
e4.mouseY=null;
}
this.RegisterSpread=function (e4){
var e7=this.GetTopSpread(e4);
if (e7!=e4)return ;
if (this.spreads==null){
this.spreads=new Array();
}
var e8=this.spreads.length;
for (var e9=0;e9<e8;e9++){
if (this.spreads[e9]==e4)return ;
}
this.spreads.length=e8+1;
this.spreads[e8]=e4;
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
e4.c1=document.createElement("XML");
e4.c1.id=e4.id+"_XMLDATA";
e4.c1.style.display="none";
document.body.insertBefore(e4.c1,null);
}
var f0=document.getElementById(e4.id+"_data");
if (f0!=null&&f0.getAttribute("data")!=null){
e4.c1.innerHTML=f0.getAttribute("data");
f0.value="";
}
this.SaveData(e4);
e4.c2=document.getElementById(e4.id+"_viewport");
if (e4.c2!=null){
e4.c3=e4.c2.parentNode;
}
e4.c4=document.getElementById(e4.id+"_corner");
if (e4.c4!=null&&e4.c4.childNodes.length>0){
e4.c4=e4.c4.getElementsByTagName("TABLE")[0];
}
e4.c5=document.getElementById(e4.id+"_rowHeader");
if (e4.c5!=null)e4.c5=e4.c5.getElementsByTagName("TABLE")[0];
e4.c6=document.getElementById(e4.id+"_colHeader");
if (e4.c6!=null)e4.c6=e4.c6.getElementsByTagName("TABLE")[0];
e4.colFooter=document.getElementById(e4.id+"_colFooter");
if (e4.colFooter!=null)e4.colFooter=e4.colFooter.getElementsByTagName("TABLE")[0];
e4.footerCorner=document.getElementById(e4.id+"_footerCorner");
if (e4.footerCorner!=null&&e4.footerCorner.childNodes.length>0){
e4.footerCorner=e4.footerCorner.getElementsByTagName("TABLE")[0];
}
var c7=e4.c7=document.getElementById(e4.id+"_commandBar");
var f1=this.GetViewport(e4);
if (f1!=null){
e4.setAttribute("rowCount",f1.rows.length);
if (f1.rows.length==1)e4.setAttribute("rowCount",0);
e4.setAttribute("colCount",f1.getAttribute("cols"));
}
var d9=e4.d9;
var e1=e4.e1;
var e0=e4.e0;
var f2=e4.footerSpanCells;
this.InitSpan(e4,this.GetViewport(e4),d9);
this.InitSpan(e4,this.GetColHeader(e4),e1);
this.InitSpan(e4,this.GetRowHeader(e4),e0);
e4.style.overflow="hidden";
if (this.GetParentSpread(e4)==null){
this.LoadScrollbarState(e4);
var f3=this.GetData(e4);
if (f3!=null){
var f4=f3.getElementsByTagName("root")[0];
var f5=f4.getElementsByTagName("activespread")[0];
if (f5!=null&&f5.innerHTML!=""){
this.SetPageActiveSpread(document.getElementById(this.Trim(f5.innerHTML)));
}
}
}
this.InitLayout(e4);
e4.e2=true;
if (this.GetPageActiveSpread()==e4&&(e4.getAttribute("AllowInsert")=="false"||e4.getAttribute("IsNewRow")=="true")){
var f6=this.GetCmdBtn(e4,"Insert");
this.UpdateCmdBtnState(f6,true);
f6=this.GetCmdBtn(e4,"Add");
this.UpdateCmdBtnState(f6,true);
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
this.SyncColSelection(e4);
}
this.Dispose=function (e4){
if (this.handlers==null)
this.handlers=new FpSpread_EventHandlers();
this.handlers.DetachEvents(e4);
}
this.CmdbarMouseOver=function (event){
var f7=this.GetTarget(event);
if (f7!=null&&f7.tagName=="IMG"&&f7.getAttribute("disabled")!="true"){
f7.style.backgroundColor="cyan";
}
}
this.CmdbarMouseOut=function (event){
var f7=this.GetTarget(event);
if (f7!=null&&f7.tagName=="IMG"){
f7.style.backgroundColor="";
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
var e7=this.GetTopSpread(e4);
var f8=document.getElementById(e7.id+"_textBox");
if (f8!=null&&f8.value!=""){
f8.value="";
}
}
this.IsXHTML=function (e4){
var e7=this.GetTopSpread(e4);
if (e7==null)return false;
var f9=e7.getAttribute("strictMode");
return (f9!=null&&f9=="true");
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
var g0=document.createEvent("Events")
g0.initEvent(name,true,true);
return g0;
}
this.Refresh=function (e4){
var f7=e4.style.display;
e4.style.display="none";
e4.style.display=f7;
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
e4.SetSelectedRange=function (r,c,rc,cc){e3.SetSelectedRange(this,r,c,rc,cc);}
e4.GetSelectedRanges=function (){return e3.GetSelectedRanges(this);}
e4.AddSelection=function (r,c,rc,cc){e3.AddSelection(this,r,c,rc,cc);}
e4.AddSpan=function (r,c,rc,cc,spans){e3.AddSpan(this,r,c,rc,cc,spans);}
e4.RemoveSpan=function (r,c,spans){e3.RemoveSpan(this,r,c,spans);}
e4.GetActiveRow=function (){var f7=e3.GetRowFromCell(this,this.d1);if (f7<0)return f7;return e3.GetSheetIndex(this,f7);}
e4.GetActiveCol=function (){return e3.GetColFromCell(this,this.d1);}
e4.SetActiveCell=function (r,c){e3.SetActiveCell(this,r,c);}
e4.GetCellByRowCol=function (r,c){return e3.GetCellByRowCol(this,r,c);}
e4.GetValue=function (r,c){return e3.GetValue(this,r,c);}
e4.SetValue=function (r,c,v,noEvent,recalc){e3.SetValue(this,r,c,v,noEvent,recalc);}
e4.GetFormula=function (r,c){return e3.GetFormula(this,r,c);}
e4.SetFormula=function (r,c,f,recalc,clientOnly){e3.SetFormula(this,r,c,f,recalc,clientOnly);}
e4.GetHiddenValue=function (r,colName){return e3.GetHiddenValue(this,r,colName);}
e4.GetSheetRowIndex=function (r){return e3.GetSheetRowIndex(this,r);}
e4.GetSheetColIndex=function (c){return e3.GetSheetColIndex(this,c);}
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
e4.GetSpread=function (f7){return e3.GetSpread(f7);}
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
e4.ShowMessage=function (msg,r,c,time){return e3.ShowMessage(this,msg,r,c,time);}
e4.HideMessage=function (r,c){return e3.HideMessage(this,r,c);}
e4.ProcessKeyMap=function (event){
if (this.keyMap!=null){
var e8=this.keyMap.length;
for (var e9=0;e9<e8;e9++){
var g1=this.keyMap[e9];
if (event.keyCode==g1.key&&event.ctrlKey==g1.ctrl&&event.shiftKey==g1.shift&&event.altKey==g1.alt){
var g2=false;
if (typeof(g1.action)=="function")
g2=g1.action();
else 
g2=eval(g1.action);
return g2;
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
var e7=this.GetTopSpread(e4);
if (e7==null)return ;
var f8=document.getElementById(e7.id+"_textBox");
if (f8==null)
{
f8=document.createElement('INPUT');
f8.type="text";
f8.setAttribute("autocomplete","off");
f8.style.position="absolute";
f8.style.borderWidth=0;
f8.style.top="-10px";
f8.style.left="-100px";
f8.style.width="0px";
f8.style.height="1px";
if (e4.tabIndex!=null)
f8.tabIndex=e4.tabIndex;
f8.id=e4.id+"_textBox";
e4.insertBefore(f8,e4.firstChild);
}
}
this.CreateLineBorder=function (e4,id){
var g3=document.getElementById(id);
if (g3==null)
{
g3=document.createElement('div');
g3.style.position="absolute";
g3.style.left="-1000px";
g3.style.top="0px";
g3.style.overflow="hidden";
g3.style.border="1px solid black";
if (e4.getAttribute("FocusBorderColor")!=null)
g3.style.borderColor=e4.getAttribute("FocusBorderColor");
if (e4.getAttribute("FocusBorderStyle")!=null)
g3.style.borderStyle=e4.getAttribute("FocusBorderStyle");
g3.id=id;
var g4=this.GetViewport(e4).parentNode;
g4.insertBefore(g3,null);
}
return g3;
}
this.CreateFocusBorder=function (e4){
if (this.GetTopSpread(e4).getAttribute("hierView")=="true")return ;
if (this.GetTopSpread(e4).getAttribute("showFocusRect")=="false")return ;
if (this.GetViewport(e4)==null)return ;
var g3=this.CreateLineBorder(e4,e4.id+"_focusRectT");
g3.style.height=0;
g3=this.CreateLineBorder(e4,e4.id+"_focusRectB");
g3.style.height=0;
g3=this.CreateLineBorder(e4,e4.id+"_focusRectL");
g3.style.width=0;
g3=this.CreateLineBorder(e4,e4.id+"_focusRectR");
g3.style.width=0;
}
this.GetPosIndicator=function (e4){
var g5=e4.posIndicator;
if (g5==null)
g5=this.CreatePosIndicator(e4);
else if (g5.parentNode!=e4)
e4.insertBefore(g5,null);
return g5;
}
this.CreatePosIndicator=function (e4){
var g5=document.createElement("img");
g5.style.position="absolute";
g5.style.top="0px";
g5.style.left="-400px";
g5.style.width="10px";
g5.style.height="10px";
g5.style.zIndex=1000;
g5.id=e4.id+"_posIndicator";
if (e4.getAttribute("clienturl")!=null)
g5.src=e4.getAttribute("clienturl")+"down.gif";
else 
g5.src=e4.getAttribute("clienturlres");
e4.insertBefore(g5,null);
e4.posIndicator=g5;
return g5;
}
this.InitSpan=function (e4,e5,spans){
if (e5!=null){
var g6=0;
if (e5==this.GetViewport(e4))
g6=e5.rows.length;
var g7=e5.rows;
var g8=this.GetColCount(e4);
for (var g9=0;g9<g7.length;g9++){
if (this.IsChildSpreadRow(e4,e5,g9)){
if (e5==this.GetViewport(e4))g6--;
}else {
var h0=g7[g9].cells;
for (var h1=0;h1<h0.length;h1++){
var h2=h0[h1];
if (h2!=null&&((h2.rowSpan!=null&&h2.rowSpan>1)||(h2.colSpan!=null&&h2.colSpan>1))){
var h3=this.GetRowFromCell(e4,h2);
var h4=parseInt(h2.getAttribute("scol"));
if (h4<g8){
this.AddSpan(e4,h3,h4,h2.rowSpan,h2.colSpan,spans);
}
}
}
}
}
if (e5==this.GetViewport(e4))e4.setAttribute("rowCount",g6);
}
}
this.GetColWithSpan=function (e4,g9,spans,h1){
var h5=0;
var h6=0;
if (h1==0){
while (this.IsCovered(e4,g9,h6,spans))
{
h6++;
}
}
for (var e9=0;e9<spans.length;e9++){
if (spans[e9].rowCount>1&&(spans[e9].col<=h1||h1==0&&spans[e9].col<h6)&&g9>=spans[e9].row&&g9<spans[e9].row+spans[e9].rowCount)
h5+=spans[e9].colCount;
}
return h5;
}
this.AddSpan=function (e4,g9,h1,rc,g8,spans){
if (spans==null)spans=e4.d9;
var h7=new this.Range();
this.SetRange(h7,"Cell",g9,h1,rc,g8);
spans.push(h7);
this.PaintFocusRect(e4);
}
this.RemoveSpan=function (e4,g9,h1,spans){
if (spans==null)spans=e4.d9;
for (var e9=0;e9<spans.length;e9++){
var h7=spans[e9];
if (h7.row==g9&&h7.col==h1){
var h8=spans.length-1;
for (var h9=e9;h9<h8;h9++){
spans[h9]=spans[h9+1];
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
var i0=this.GetOperationMode(e4);
if (e4.d1==null&&i0!="MultiSelect"&&i0!="ExtendedSelect"&&e4.GetRowCount()>0&&e4.GetColCount()>0){
var i1=this.FireActiveCellChangingEvent(e4,0,0,0);
if (!i1){
e4.SetActiveCell(0,0);
var g0=this.CreateEvent("ActiveCellChanged");
g0.cmdID=e4.id;
g0.row=g0.Row=0;
g0.col=g0.Col=0;
if (e4.getAttribute("LayoutMode"))
g0.InnerRow=g0.innerRow=0;
this.FireEvent(e4,g0);
}
}
var e7=this.GetTopSpread(e4);
var f8=document.getElementById(e7.id+"_textBox");
if (e4.d1!=null){
var i2=this.GetEditor(e4.d1);
if (i2==null){
if (f8!=null){
if (this.b8!=f8){
try {f8.focus();}catch (g0){}
}
}
}else {
if (i2.tagName!="SELECT")i2.focus();
this.SetEditorFocus(i2);
}
}else {
if (f8!=null){
try {f8.focus();}catch (g0){}
}
}
this.EnableButtons(e4);
}
this.GetTotalRowCount=function (e4){
var f7=parseInt(e4.getAttribute("totalRowCount"));
if (isNaN(f7))f7=0;
return f7;
}
this.GetPageCount=function (e4){
var f7=parseInt(e4.getAttribute("pageCount"));
if (isNaN(f7))f7=0;
return f7;
}
this.GetColCount=function (e4){
var f7=parseInt(e4.getAttribute("colCount"));
if (isNaN(f7))f7=0;
return f7;
}
this.GetRowCount=function (e4){
var f7=parseInt(e4.getAttribute("rowCount"));
if (isNaN(f7))f7=0;
return f7;
}
this.GetRowCountInternal=function (e4){
var f7=parseInt(this.GetViewport(e4).rows.length);
if (isNaN(f7))f7=0;
return f7;
}
this.IsChildSpreadRow=function (e4,view,g9){
if (e4==null||view==null)return false;
if (g9>=1&&g9<view.rows.length){
if (view.rows[g9].cells.length>0&&view.rows[g9].cells[0]!=null&&view.rows[g9].cells[0].firstChild!=null){
var f7=view.rows[g9].cells[0].firstChild;
if (f7.nodeName!="#text"&&f7.getAttribute("FpSpread")=="Spread")return true;
}
}
return false;
}
this.GetChildSpread=function (e4,row,rindex){
var i3=this.GetViewport(e4);
if (i3!=null){
var g9=this.GetDisplayIndex(e4,row)+1;
if (typeof(rindex)=="number")g9+=rindex;
if (g9>=1&&g9<i3.rows.length){
if (i3.rows[g9].cells.length>0&&i3.rows[g9].cells[0]!=null&&i3.rows[g9].cells[0].firstChild!=null){
var f7=i3.rows[g9].cells[0].firstChild;
if (f7.nodeName!="#text"&&f7.getAttribute("FpSpread")=="Spread"){
return f7;
}
}
}
}
return null;
}
this.GetChildSpreads=function (e4){
var e9=0;
var g2=new Array();
var i3=this.GetViewport(e4);
if (i3!=null){
for (var g9=1;g9<i3.rows.length;g9++){
if (i3.rows[g9].cells.length>0&&i3.rows[g9].cells[0]!=null&&i3.rows[g9].cells[0].firstChild!=null){
var f7=i3.rows[g9].cells[0].firstChild;
if (f7.nodeName!="#text"&&f7.getAttribute("FpSpread")=="Spread"){
g2.length=e9+1;
g2[e9]=f7;
e9++;
}
}
}
}
return g2;
}
this.GetDisplayIndex=function (e4,row){
if (row<0)return -1;
var e9=0;
var g9=0;
var i3=this.GetViewport(e4);
if (i3!=null){
for (e9=0;e9<i3.rows.length;e9++){
if (this.IsChildSpreadRow(e4,i3,e9))continue ;
if (g9==row)break ;
g9++;
}
}
return e9;
}
this.GetSheetIndex=function (e4,row,c2){
var e9=0
var g9=0;
var i3=c2;
if (i3==null)i3=this.GetViewport(e4);
if (i3!=null){
if (row<0||row>=i3.rows.length)return -1;
for (e9=0;e9<row;e9++){
if (this.IsChildSpreadRow(e4,i3,e9))continue ;
g9++;
}
}
return g9;
}
this.GetParentRowIndex=function (e4){
var i4=this.GetParentSpread(e4);
if (i4==null)return -1;
var i3=this.GetViewport(i4);
if (i3==null)return -1;
var i5=e4.parentNode.parentNode;
var e9=i5.rowIndex-1;
for (;e9>0;e9--){
if (this.IsChildSpreadRow(i4,i3,e9))continue ;
else 
break ;
}
return this.GetSheetIndex(i4,e9,i3);
}
this.CreateTestBox=function (e4){
var i6=document.getElementById(e4.id+"_testBox");
if (i6==null)
{
i6=document.createElement("span");
i6.style.position="absolute";
i6.style.borderWidth=0;
i6.style.top="-500px";
i6.style.left="-100px";
i6.id=e4.id+"_testBox";
e4.insertBefore(i6,e4.firstChild);
}
return i6;
}
this.SizeToFit=function (e4,h1){
if (h1==null||h1<0)h1=0;
var e5=this.GetViewport(e4);
if (e5!=null){
var i6=this.CreateTestBox(e4);
var g7=e5.rows;
var i7=0;
for (var g9=0;g9<g7.length;g9++){
if (!this.IsChildSpreadRow(e4,e5,g9)){
var i8=this.GetCellFromRowCol(e4,g9,h1);
if (i8.colSpan>1)continue ;
var i9=this.GetPreferredCellWidth(e4,i8,i6);
if (i9>i7)i7=i9;
}
}
this.SetColWidth(e4,h1,i7);
}
}
this.GetPreferredCellWidth=function (e4,i8,i6){
if (i6==null)i6=this.CreateTestBox(e4);
var j0=this.GetRender(e4,i8);
var j1=this.GetCellType(i8);
var j2=this.GetEditor(i8);
if (j0!=null){
i6.style.fontFamily=j0.style.fontFamily;
i6.style.fontSize=j0.style.fontSize;
i6.style.fontWeight=j0.style.fontWeight;
i6.style.fontStyle=j0.style.fontStyle;
}
if (j0!=null&&j1=="MultiColumnComboBoxCellType"){
var j3=i8.getElementsByTagName("Table")[0];
if (j3!=null){
i6.innerHTML=this.GetEditorValue(j2)+"123";
}
}
else {
i6.innerHTML=i8.innerHTML;
}
var i9=i6.offsetWidth+8;
if (i8.style.paddingLeft!=null&&i8.style.paddingLeft.length>0)
i9+=parseInt(i8.style.paddingLeft);
if (i8.style.paddingRight!=null&&i8.style.paddingRight.length>0)
i9+=parseInt(i8.style.paddingRight);
return i9;
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
this.SynRowHeight=function (e4,c5,e5,g9,updateParent,header,c4){
if (c5==null||e5==null)return ;
if (typeof(c5.rows[g9])!="undefined"&&
typeof(c5.rows[g9].cells[0])!="undefined")
{
if (c5.rows[g9].cells[0].style.posHeight==null||c5.rows[g9].cells[0].style.posHeight=="")
c5.rows[g9].cells[0].style.posHeight=c5.rows[g9].offsetHeight-1;
}
var j4=c5.rows[g9].offsetHeight;
var g4=e5.rows[g9].offsetHeight;
if (j4==g4&&(g9>0||c4))return ;
var j5=0;
if (e5.cellSpacing=="0"&&g9==0){
if (document.defaultView!=null&&document.defaultView.getComputedStyle!=null){
var j6=0;
for (var e9=0;e9<e5.rows[g9].cells.length;e9++){
j6=parseInt(document.defaultView.getComputedStyle(e5.rows[g9].cells[e9],'').getPropertyValue("border-top-width"));
if (j6>j5)j5=j6;
}
}
}
e5.rows[g9].style.height="";
var j7=Math.max(j4,g4);
j5=parseInt(j5/2);
var j8=this.IsXHTML(e4);
if (this.IsChildSpreadRow(e4,e5,g9)){
if (j8)j7-=1;
c5.rows[g9].cells[0].style.posHeight=j7-1;
return ;
}
if (j8){
if (j7==j4){
if (e5.rows[g9].cells[0]!=null){
if (e5.cellSpacing=="0"&&g9==0){
e5.rows[g9].cells[0].style.posHeight+=(j7-g4-j5);
}else {
e5.rows[g9].cells[0].style.posHeight+=(j7-g4);
}
}
}else {
if (c5.rows[g9].cells[0]!=null){
if (c5.cellSpacing=="0"&&g9==0){
c5.rows[g9].cells[0].style.posHeight+=(j7-j4+j5);
}else {
c5.rows[g9].cells[0].style.posHeight+=(j7-j4);
}
}
}
}
else 
{
if (j7==j4){
if (e5.rows[g9].cells[0]!=null){
if (g9==0&&parseInt(e5.cellSpacing)==0){
e5.rows[g9].cells[0].style.posHeight=j4-j5;
}else {
e5.rows[g9].cells[0].style.posHeight=j4;
}
}
}
else {
if (c5.rows[g9].cells[0]!=null){
if (g9==0&&parseInt(e5.cellSpacing)==0){
c5.rows[g9].cells[0].style.posHeight=g4+j5;
}else 
c5.rows[g9].cells[0].style.posHeight=g4;
}
}
}
if (updateParent){
var i4=this.GetParentSpread(e4);
if (i4!=null)this.UpdateRowHeight(i4,e4);
}
}
this.SizeAll=function (e4){
var j9=this.GetChildSpreads(e4);
if (j9!=null&&j9.length>0){
for (var e9=0;e9<j9.length;e9++){
this.SizeAll(j9[e9]);
}
}
this.SizeSpread(e4);
if (this.GetParentSpread(e4)!=null)
this.Refresh(e4);
}
this.SizeSpread=function (e4){
var j8=this.IsXHTML(e4);
var c2=this.GetViewport(e4);
if (c2==null)return ;
this.SyncMsgs(e4);
var c5=this.GetRowHeader(e4);
if (c5!=null){
for (var e9=0;e9<c2.rows.length&&e9<c5.rows.length;e9++){
this.SynRowHeight(e4,c5,c2,e9,false,true);
this.SynRowHeight(e4,c5,c2,e9,false,true);
if (e9==0&&c5.rows[0].cells[0]&&c2.rows[0].cells[0]&&c2.rows[0].cells[0].getAttribute("CellType2")=="SlideShowCellType")
c5.rows[0].cells[0].style.posHeight=c2.rows[0].cells[0].offsetHeight-1;
}
}
var k0=this.GetColFooter(e4);
var c6=this.GetColHeader(e4);
var c4=this.GetCorner(e4);
if (c4!=null&&c6!=null&&c4.getAttribute("allowTableCorner")){
for (var e9=0;e9<c4.rows.length&&e9<c6.rows.length;e9++){
if (c6.rows[e9].cells.length){
if (c6.rows[0].cells.length>1)
this.SynRowHeight(e4,c6,c4,e9,true,true,false);
this.SynRowHeight(e4,c4,c6,e9,true,false,true);
}
}
}
var k1=this.GetColGroup(c2);
var k2=this.GetColGroup(c6);
if (k1!=null&&k1.childNodes.length>0&&k2!=null&&k2.childNodes.length>0){
var k3=-1;
if (this.b4!=null)k3=parseInt(this.b4.getAttribute("index"));
if (this.b4==null||k3==0)
{
var k4=parseInt(k1.childNodes[0].width);var k5=parseInt(k1.childNodes[0].offsetLeft);
k2.childNodes[0].width=""+(k4-k5)+"px";
k1.childNodes[0].width=""+k4+"px";
this.SetWidthFix(c6,0,(k4-k5));
this.SetWidthFix(c2,0,k4);
}
}
var i4=this.GetParentSpread(e4);
if (i4!=null)this.UpdateRowHeight(i4,e4);
var j7=e4.clientHeight;
var k6=this.GetCommandBar(e4);
if (k6!=null)
{
k6.style.width=""+e4.clientWidth+"px";
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
k6.parentNode.style.borderTop="1px solid white";
k6.parentNode.style.backgroundColor=k6.style.backgroundColor;
}
var k7=this.GetElementById(k6,e4.id+"_cmdTable");
if (k7!=null){
if (e4.style.position!="absolute"&&e4.style.position!="relative"&&(k7.style.height==""||parseInt(k7.style.height)<27)){
k7.style.height=""+(k7.offsetHeight+3)+"px";
}
if (!j8&&parseInt(c2.cellSpacing)>0)
k7.parentNode.style.height=""+(k7.offsetHeight+3)+"px";
j7-=Math.max(k7.parentNode.offsetHeight,k7.offsetHeight);
}
if (e4.style.position!="absolute"&&e4.style.position!="relative")
k6.style.position="";
}
var c6=this.GetColHeader(e4);
if (c6!=null)
{
j7-=c6.offsetHeight;
c6.parentNode.style.height=""+(c6.offsetHeight-parseInt(c6.cellSpacing))+"px";
if (j8)
j7+=parseInt(c6.cellSpacing);
}
var k0=this.GetColFooter(e4);
if (k0!=null)
{
j7-=k0.offsetHeight;
k0.parentNode.style.height=""+(k0.offsetHeight)+"px";
}
var c8=this.GetHierBar(e4);
if (c8!=null)
{
j7-=c8.offsetHeight;
}
var k8=document.getElementById(e4.id+"_titleBar");
if (k8)j7-=k8.parentNode.parentNode.offsetHeight;
var k9=this.GetGroupBar(e4);
if (k9!=null){
j7-=k9.offsetHeight;
}
var c9=this.GetPager1(e4);
if (c9!=null)
{
j7-=c9.offsetHeight;
this.InitSlideBar(e4,c9);
}
var l0=(e4.getAttribute("cmdTop")=="true");
var d0=this.GetPager2(e4);
if (d0!=null)
{
d0.style.width=""+(e4.clientWidth-10)+"px";
j7-=Math.max(d0.offsetHeight,28);
this.InitSlideBar(e4,d0);
}
var l1=null;
if (c5!=null)l1=c5.parentNode;
var l2=null;
if (c6!=null)l2=c6.parentNode;
var l3=null;
if (k0!=null)l3=k0.parentNode;
var l4=this.GetFooterCorner(e4);
if (l3!=null)
{
l3.style.height=""+k0.offsetHeight-parseInt(c2.cellSpacing)+"px";
if (l4!=null){
l4.parentNode.style.height=l3.style.height;
}
}
if (l4!=null&&!j8)
l4.width=""+(l4.parentNode.offsetWidth+parseInt(c2.cellSpacing))+"px";
var l5=c2.parentNode;
var c4=this.GetCorner(e4);
if (j8&&l2!=null)
{
l2.style.height=""+c6.offsetHeight-parseInt(c2.cellSpacing)+"px";
if (c4!=null){
c4.parentNode.style.height=l2.style.height;
}
}
if (c4!=null&&!j8)
c4.width=""+(c4.parentNode.offsetWidth+parseInt(c2.cellSpacing))+"px";
if (l5!=null){
if (l1!=null){
l5.style.width=""+Math.max(e4.clientWidth-l1.offsetWidth+parseInt(c2.cellSpacing),1)+"px";
l5.style.height=""+Math.max(j7,1)+"px";
l5.style.width=""+Math.max(e4.clientWidth-l1.offsetWidth+parseInt(c2.cellSpacing),1)+"px";
}else {
l5.style.width=""+Math.max(e4.clientWidth,1)+"px";
l5.style.height=""+Math.max(j7,1)+"px";
l5.style.width=""+Math.max(e4.clientWidth,1)+"px";
}
}
var l6=0;
if (this.GetColFooter(e4)){
l6=this.GetColFooter(e4).offsetHeight;
}
if (k6!=null&&!l0){
if (d0!=null){
if (e4.style.position=="absolute"||e4.style.position=="relative"){
k6.style.position="absolute";
k6.style.top=""+(e4.clientHeight-Math.max(d0.offsetHeight,28)-k6.offsetHeight)+"px";
}else {
k6.style.position="absolute";
k6.style.top=""+(c2.parentNode.offsetTop+l6+c2.parentNode.offsetHeight)+"px";
}
}else {
if (e4.style.position=="absolute"||e4.style.position=="relative")
{
k6.style.position="absolute";
k6.style.top=""+(e4.clientHeight-k6.offsetHeight)+"px";
}else {
k6.style.position="absolute";
if (d0!=null)
k6.style.top=""+(this.GetOffsetTop(e4,e4,document.body)+e4.clientHeight-Math.max(d0.offsetHeight,28)-k6.offsetHeight)+"px";
else 
k6.style.top=""+(this.GetOffsetTop(e4,e4,document.body)+e4.clientHeight-k6.offsetHeight+1)+"px";
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
if (k6!=null&&!l0)
d0.style.top=""+(c2.parentNode.offsetTop+c2.parentNode.offsetHeight+k6.offsetHeight+l6)+"px";
else 
d0.style.top=""+(c2.parentNode.offsetTop+c2.parentNode.offsetHeight+l6)+"px";
}
}
if (l1!=null){
if (j8)l1.style.height=""+Math.max(l5.offsetHeight,1)+"px";
else l1.style.height=Math.max(l5.offsetHeight,1);
}
if (c2&&!c5){
c2.parentNode.parentNode.style.height=""+c2.parentNode.offsetHeight+"px";
}
return ;
if (this.GetParentSpread(e4)==null&&l2!=null){
var i9=0;
if (l1!=null){
i9=Math.max(e4.clientWidth-l1.offsetWidth,1);
}else {
i9=Math.max(e4.clientWidth,1);
}
l2.style.width=i9;
l2.parentNode.style.width=i9;
}
if (j8)
{
if (c2!=null){
c2.style.posTop=-c2.cellSpacing;
var l7=e4.clientWidth;
if (c5!=null)l7-=c5.parentNode.offsetWidth;
c2.parentNode.style.width=""+l7+"px";
}
if (c5!=null){
c5.style.position="relative";
c5.parentNode.style.position="relative";
c5.style.posTop=-c2.cellSpacing;
c5.width=""+(c5.parentNode.offsetWidth)+"px";
}
}else {
if (c2!=null){
var l7=e4.clientWidth;
if (c5!=null){
l7-=c5.parentNode.offsetWidth;
c5.width=""+(c5.parentNode.offsetWidth+parseInt(c2.cellSpacing))+"px";
}
c2.parentNode.style.width=""+l7+"px";
}
}
this.ScrollView(e4);
this.PaintFocusRect(e4);
}
this.InitSlideBar=function (e4,pager){
var l8=this.GetElementById(pager,e4.id+"_slideBar");
if (l8!=null){
var j8=this.IsXHTML(e4);
if (j8)
l8.style.height=Math.max(pager.offsetHeight,28)+"px";
else 
l8.style.height=(pager.offsetHeight-2)+"px";
var f7=pager.getElementsByTagName("TABLE");
if (f7!=null&&f7.length>0){
var l9=f7[0].rows[0];
var h4=l9.cells[0];
var m0=l9.cells[2];
e4.slideLeft=Math.max(107,h4.offsetWidth+1);
if (h4.style.paddingRight!="")e4.slideLeft+=parseInt(h4.style.paddingRight);
e4.slideRight=pager.offsetWidth-m0.offsetWidth-l8.offsetWidth-3;
if (m0.style.paddingRight!="")e4.slideRight-=parseInt(m0.style.paddingLeft);
var m1=parseInt(pager.getAttribute("curPage"));
var m2=parseInt(pager.getAttribute("totalPage"))-1;
if (m2==0)m2=1;
var m3=false;
var l7=Math.max(107,e4.slideLeft)+(m1/m2)*(e4.slideRight-e4.slideLeft);
if (pager.id.indexOf("pager1")>=0&&e4.style.position!="absolute"&&e4.style.position!="relative"){
l7+=this.GetOffsetLeft(e4,pager,document);
var m4=(this.GetOffsetTop(e4,h4,pager)+this.GetOffsetTop(e4,pager,document));
l8.style.top=m4+"px";
m3=true;
}
var k8=document.getElementById(e4.id+"_titleBar");
if (pager.id.indexOf("pager1")>=0&&!m3&&k8!=null){
var m4=k8.parentNode.parentNode.offsetHeight;
l8.style.top=m4+"px";
}
l8.style.left=l7+"px";
}
}
}
this.InitLayout=function (e4){
this.SizeSpread(e4);
this.SizeSpread(e4);
this.SizeSpread(e4);
}
this.GetRowByKey=function (e4,key){
if (key=="-1")
return -1;
var m5=this.GetViewport(e4);
if (m5!=null){
for (var i5=0;i5<m5.rows.length;i5++){
if (m5.rows[i5].getAttribute("FpKey")==key){
return i5;
}
}
}
if (m5!=null)
return 0;
else 
return -1;
}
this.GetColByKey=function (e4,key){
if (key=="-1")
return -1;
var m5=this.GetViewport(e4);
var m6=this.GetColGroup(m5);
if (m6==null||m6.childNodes.length==0)
m6=this.GetColGroup(this.GetColHeader(e4));
if (m6!=null){
for (var m7=0;m7<m6.childNodes.length;m7++){
var f7=m6.childNodes[m7];
if (f7.getAttribute("FpCol")==key){
return m7;
}
}
}
return 0;
}
this.IsRowSelected=function (e4,i5){
var m8=this.GetSelection(e4);
if (m8!=null){
var m9=m8.firstChild;
while (m9!=null){
var g9=parseInt(m9.getAttribute("rowIndex"));
var n0=parseInt(m9.getAttribute("rowcount"));
if (g9<=i5&&i5<g9+n0)
return true;
m9=m9.nextSibling;
}
}
}
this.InitSelection=function (e4){
var g9=0;
var h1=0;
var f3=this.GetData(e4);
if (f3==null)return ;
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var m8=n1.getElementsByTagName("selection")[0];
var n2=n1.firstChild;
while (n2!=null&&n2.tagName!="activerow"&&n2.tagName!="ACTIVEROW"){
n2=n2.nextSibling;
}
if (n2!=null)
g9=this.GetRowByKey(e4,n2.innerHTML);
if (g9>=this.GetRowCount(e4))g9=0;
var n3=n1.firstChild;
while (n3!=null&&n3.tagName!="activecolumn"&&n3.tagName!="ACTIVECOLUMN"){
n3=n3.nextSibling;
}
if (n3!=null)
h1=this.GetColByKey(e4,n3.innerHTML);
if (g9<0)g9=0;
if (g9>=0||h1>=0){
var n4=f3;
if (this.GetParentSpread(e4)!=null){
var n5=this.GetTopSpread(e4);
if (n5.initialized)n4=this.GetData(n5);
f4=n4.getElementsByTagName("root")[0];
}
var n6=f4.getElementsByTagName("activechild")[0];
e4.d3=g9;e4.d4=h1;
if ((this.GetParentSpread(e4)==null&&(n6==null||n6.innerHTML==""))||(n6!=null&&e4.id==this.Trim(n6.innerHTML))){
this.UpdateAnchorCell(e4,g9,h1);
}else {
e4.d1=this.GetCellFromRowCol(e4,g9,h1);
}
}
var m9=m8.firstChild;
while (m9!=null){
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
m9.setAttribute("rowIndex",g9);
m9.setAttribute("colIndex",h1);
this.PaintSelection(e4,g9,h1,n0,g8,true);
m9=m9.nextSibling;
}
this.PaintFocusRect(e4);
}
this.TranslateKey=function (event){
event=this.GetEvent(event);
var n7=this.GetTarget(event);
try {
if (document.readyState!=null&&document.readyState!="complete")return ;
var e4=this.GetPageActiveSpread();
if (event.altKey&&event.keyCode==this.down&&typeof(n7.getAttribute("mccbparttype"))!="undefined"&&n7.getAttribute("mccbparttype")=="DropDownButton")return ;
if (typeof(e4.getAttribute("mcctCellType"))!="undefined"&&e4.getAttribute("mcctCellType")=="true")return ;
if (this.GetOperationMode(e4)=="RowMode"&&this.GetEnableRowEditTemplate(e4)=="true"&&this.IsInRowEditTemplate(e4,n7))return ;
if (e4!=null){
if (event.keyCode==229){
this.CancelDefault(event);
return ;
}
if (!this.IsChild(n7,this.GetTopSpread(e4)))return ;
this.KeyDown(e4,event);
var n8=false;
if (event.keyCode==this.tab){
var n9=this.GetProcessTab(e4);
n8=(n9=="true"||n9=="True");
}
if (n8)
this.CancelDefault(event);
}
}catch (g0){}
}
this.IsInRowEditTemplate=function (e4,n7){
while (n7&&n7.parentNode){
n7=n7.parentNode;
if (n7.tagName=="DIV"&&n7.id==e4.id+"_RowEditTemplateContainer")
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
var e8=e4.keyMap.length;
for (var e9=0;e9<e8;e9++){
var g1=e4.keyMap[e9];
if (g1!=null&&g1.key==keyCode&&g1.ctrl==ctrl&&g1.shift==shift&&g1.alt==alt){
for (var h9=e9+1;h9<e8;h9++){
e4.keyMap[h9-1]=e4.keyMap[h9];
}
e4.keyMap.length=e4.keyMap.length-1;
break ;
}
}
}
this.AddKeyMap=function (e4,keyCode,ctrl,shift,alt,action){
if (e4.keyMap==null)e4.keyMap=new Array();
var g1=this.GetKeyAction(e4,keyCode,ctrl,shift,alt);
if (g1!=null){
g1.action=action;
}else {
var e8=e4.keyMap.length;
e4.keyMap.length=e8+1;
e4.keyMap[e8]=new this.KeyAction(keyCode,ctrl,shift,alt,action);
}
}
this.GetKeyAction=function (e4,keyCode,ctrl,shift,alt){
if (e4.keyMap==null)e4.keyMap=new Array();
var e8=e4.keyMap.length;
for (var e9=0;e9<e8;e9++){
var g1=e4.keyMap[e9];
if (g1!=null&&g1.key==keyCode&&g1.ctrl==ctrl&&g1.shift==shift&&g1.alt==alt){
return g1;
}
}
return null;
}
this.MoveToPrevCell=function (e4){
var o0=this.EndEdit(e4);
if (!o0)return ;
var g9=e4.GetActiveRow();
var h1=e4.GetActiveCol();
this.MoveLeft(e4,g9,h1);
}
this.MoveToNextCell=function (e4){
var o0=this.EndEdit(e4);
if (!o0)return ;
var g9=e4.GetActiveRow();
var h1=e4.GetActiveCol();
this.MoveRight(e4,g9,h1);
}
this.MoveToNextRow=function (e4){
var o0=this.EndEdit(e4);
if (!o0)return ;
var g9=e4.GetActiveRow();
var h1=e4.GetActiveCol();
this.MoveDown(e4,g9,h1);
}
this.MoveToPrevRow=function (e4){
var o0=this.EndEdit(e4);
if (!o0)return ;
var g9=e4.GetActiveRow();
var h1=e4.GetActiveCol();
if (g9>0)
this.MoveUp(e4,g9,h1);
}
this.MoveToFirstColumn=function (e4){
var o0=this.EndEdit(e4);
if (!o0)return ;
var g9=e4.GetActiveRow();
if (e4.d1.parentNode.rowIndex>=0)
this.UpdateLeadingCell(e4,g9,0);
}
this.MoveToLastColumn=function (e4){
var o0=this.EndEdit(e4);
if (!o0)return ;
var g9=e4.GetActiveRow();
if (e4.d1.parentNode.rowIndex>=0){
h1=this.GetColCount(e4)-1;
this.UpdateLeadingCell(e4,g9,h1);
}
}
this.UpdatePostbackData=function (e4){
this.SaveData(e4);
}
this.PrepareData=function (m9){
var g2="";
if (m9!=null){
if (m9.nodeName=="#text")
g2=m9.nodeValue;
else {
g2=this.GetBeginData(m9);
var f7=m9.firstChild;
while (f7!=null){
var o1=this.PrepareData(f7);
if (o1!="")g2+=o1;
f7=f7.nextSibling;
}
g2+=this.GetEndData(m9);
}
}
return g2;
}
this.GetBeginData=function (m9){
var g2="<"+m9.nodeName.toLowerCase();
if (m9.attributes!=null){
for (var e9=0;e9<m9.attributes.length;e9++){
var o2=m9.attributes[e9];
if (o2.nodeName!=null&&o2.nodeName!=""&&o2.nodeName!="style"&&o2.nodeValue!=null&&o2.nodeValue!="")
g2+=(" "+o2.nodeName+"=\""+o2.nodeValue+"\"");
}
}
g2+=">";
return g2;
}
this.GetEndData=function (m9){
return "</"+m9.nodeName.toLowerCase()+">";
}
this.SaveData=function (e4){
if (e4==null)return ;
try {
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var f7=this.PrepareData(f4);
var o3=document.getElementById(e4.id+"_data");
o3.value=encodeURIComponent(f7);
}catch (g0){
alert("e "+g0);
}
}
this.SetActiveSpread=function (event){
try {
event=this.GetEvent(event);
var n7=this.GetTarget(event);
var o4=this.GetSpread(n7,false);
var o5=this.GetPageActiveSpread();
if (this.a7&&(o4==null||(o4!=o5&&o4.getAttribute("mcctCellType")!="true"&&o5.getAttribute("mcctCellType")!="true"))){
if (n7!=this.a8&&this.a8!=null){
if (this.a8.blur!=null)this.a8.blur();
}
var o0=this.EndEdit();
if (!o0)return ;
}
var o6=false;
if (o4==null){
o4=this.GetSpread(n7,true);
o6=(o4!=null);
}
var h2=this.GetCell(n7,true);
if (h2==null&&o5!=null&&o5.e2){
this.SaveData(o5);
o5.e2=false;
}
if (o5!=null&&o5.e2&&(o4!=o5||o4==null||o6)){
this.SaveData(o5);
o5.e2=false;
}
if (o5!=null&&o5.e2&&o4==o5&&n7.tagName=="INPUT"&&(n7.type=="submit"||n7.type=="button"||n7.type=="image")){
this.SaveData(o5);
o5.e2=false;
}
if (o4!=null&&this.GetOperationMode(o4)=="ReadOnly")return ;
var n5=null;
if (o4==null){
if (o5==null)return ;
n5=this.GetTopSpread(o5);
this.SetActiveSpreadID(n5,"",null,false);
this.SetPageActiveSpread(null);
}else {
if (o4!=o5){
if (o5!=null){
n5=this.GetTopSpread(o5);
this.SetActiveSpreadID(n5,"",null,false);
}
if (o6){
n5=this.GetTopSpread(o4);
var j6=this.GetTopSpread(o5);
if (n5!=j6){
this.SetActiveSpreadID(n5,o4.id,o4.id,true);
this.SetPageActiveSpread(o4);
}else {
this.SetActiveSpreadID(n5,o5.id,o5.id,true);
this.SetPageActiveSpread(o5);
}
}else {
n5=this.GetTopSpread(o4);
this.SetPageActiveSpread(o4);
this.SetActiveSpreadID(n5,o4.id,o4.id,false);
}
}
}
}catch (g0){}
}
this.SetActiveSpreadID=function (e4,id,child,o6){
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var f5=f4.getElementsByTagName("activespread")[0];
var o7=f4.getElementsByTagName("activechild")[0];
if (f5==null)return ;
if (o6&&o7!=null&&o7.nodeValue!=""){
f5.innerHTML=o7.innerHTML;
}else {
f5.innerHTML=id;
if (child!=null&&o7!=null)o7.innerHTML=child;
}
this.SaveData(e4);
e4.e2=false;
}
this.GetSpread=function (ele,incCmdBar){
var i9=ele;
while (i9!=null&&i9.tagName!="BODY"){
if (typeof(i9.getAttribute)!="function")break ;
var e4=i9.getAttribute("FpSpread");
if (e4==null)e4=i9.FpSpread;
if (e4=="Spread"){
if (!incCmdBar){
var f7=ele;
while (f7!=null&&f7!=i9){
if (f7.id==i9.id+"_commandBar"||f7.id==i9.id+"_pager1"||f7.id==i9.id+"_pager2")return null;
f7=f7.parentNode;
}
}
return i9;
}
i9=i9.parentNode;
}
return null;
}
this.ScrollViewport=function (event){
var f7=this.GetTarget(event);
var e4=this.GetTopSpread(f7);
if (e4!=null)this.ScrollView(e4);
}
this.GetActiveChildSheetView=function (e4){
var o5=this.GetPageActiveSheetView();
if (typeof(o5)=="undefined")return null;
var n5=this.GetTopSpread(e4);
var o8=this.GetTopSpread(o5);
if (o8!=n5)return null;
if (o5==o8)return null;
return o5;
}
this.ScrollTo=function (e4,i5,m7){
var h2=this.GetCellByRowCol(e4,i5,m7);
if (h2==null)return ;
var i3=this.GetViewport(e4).parentNode;
if (i3==null)return ;
i3.scrollTop=h2.offsetTop;
i3.scrollLeft=h2.offsetLeft;
}
this.ScrollView=function (e4){
var o4=this.GetTopSpread(e4);
var c5=this.GetParent(this.GetRowHeader(o4));
var c6=this.GetParent(this.GetColHeader(o4));
var k0=this.GetParent(this.GetColFooter(o4));
var i3=this.GetParent(this.GetViewport(o4));
var o9=false;
if (c5!=null){
o9=(c5.scrollTop!=i3.scrollTop);
c5.scrollTop=i3.scrollTop;
}
if (c6!=null){
if (!o9)o9=(c6.scrollLeft!=i3.scrollLeft);
c6.scrollLeft=i3.scrollLeft;
}
if (k0!=null){
if (!o9)o9=(k0.scrollLeft!=i3.scrollLeft);
k0.scrollLeft=i3.scrollLeft;
}
if (this.GetParentSpread(e4)==null)this.SaveScrollbarState(e4,i3.scrollTop,i3.scrollLeft);
if (o9){
var g0=this.CreateEvent("Scroll");
this.FireEvent(e4,g0);
if (e4.frzRows!=0||e4.frzCols!=0)this.SyncMsgs(e4);
}
if (i3.scrollTop>0&&i3.scrollTop+i3.offsetHeight>=this.GetViewport(o4).offsetHeight){
if (!this.a7&&e4.getAttribute("loadOnDemand")=="true"){
if (e4.LoadState!=null)return ;
e4.LoadState=true;
this.SaveData(e4);
e4.CallBack("LoadOnDemand",true);
}
}
}
this.SaveScrollbarState=function (e4,scrollTop,scrollLeft){
if (this.GetParentSpread(e4)!=null)return ;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var p0=f4.getElementsByTagName("scrollTop")[0];
var p1=f4.getElementsByTagName("scrollLeft")[0];
if (e4.getAttribute("scrollContent")=="true")
if (p0!=null&&p1!=null)
if (p0.innerHTML!=scrollTop||p1.innerHTML!=scrollLeft)
this.ShowScrollingContent(e4,p0.innerHTML==scrollTop);
if (p0!=null)p0.innerHTML=scrollTop;
if (p1!=null)p1.innerHTML=scrollLeft;
}
this.LoadScrollbarState=function (e4){
return ;
if (this.GetParentSpread(e4)!=null)return ;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var p0=f4.getElementsByTagName("scrollTop")[0];
var p1=f4.getElementsByTagName("scrollLeft")[0];
var p2=0;
if (p0!=null&&p0.innerHTML!=""){
p2=parseInt(p0.innerHTML);
}else {
p2=0;
}
var p3=0;
if (p1!=null&&p1.innerHTML!=""){
p3=parseInt(p1.innerHTML);
}else {
p3=0;
}
var i3=this.GetParent(this.GetViewport(e4));
if (i3!=null){
if (!isNaN(p2))i3.scrollTop=p2;
if (!isNaN(p3))i3.scrollLeft=p3;
var c5=this.GetParent(this.GetRowHeader(e4));
var c6=this.GetParent(this.GetColHeader(e4));
var k0=this.GetParent(this.GetColFooter(e4));
if (k0!=null){
k0.scrollLeft=i3.scrollLeft;
}
if (c5!=null){
c5.scrollTop=i3.scrollTop;
}
if (c6!=null){
c6.scrollLeft=i3.scrollLeft;
}
}
}
this.GetParent=function (g0){
if (g0==null)
return null;
else 
return g0.parentNode;
}
this.GetViewport=function (e4){
return e4.c2;
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
this.GetColFooter=function (e4){
return e4.colFooter;
}
this.GetFooterCorner=function (e4){
return e4.footerCorner;
}
this.GetCmdBtn=function (e4,id){
var o4=this.GetTopSpread(e4);
var p4=this.GetCommandBar(o4);
if (p4!=null)
return this.GetElementById(p4,o4.id+"_"+id);
else 
return null;
}
this.Range=function (){
this.type="Cell";
this.row=-1;
this.col=-1;
this.rowCount=0;
this.colCount=0;
}
this.SetRange=function (h7,type,i5,m7,n0,g8){
h7.type=type;
h7.row=i5;
h7.col=m7;
h7.rowCount=n0;
h7.colCount=g8;
if (type=="Row"){
h7.col=h7.colCount=-1;
}else if (type=="Column"){
h7.row=h7.rowCount=-1;
}else if (type=="Table"){
h7.col=h7.colCount=-1;h7.row=h7.rowCount=-1;
}
}
this.Margin=function (left,top,right,bottom){
this.left;
this.top;
this.right;
this.bottom;
}
this.GetRender=function (h2){
var f7=h2;
if (f7.firstChild!=null&&f7.firstChild.tagName!=null&&f7.firstChild.tagName!="BR")
return f7.firstChild;
if (f7.firstChild!=null&&f7.firstChild.value!=null){
f7=f7.firstChild;
}
return f7;
}
this.GetPreferredRowHeight=function (e4,g9){
var i6=this.CreateTestBox(e4);
g9=this.GetDisplayIndex(e4,g9);
var i3=this.GetViewport(e4);
var i7=0;
var p5=i3.rows[g9].offsetHeight;
var e8=i3.rows[g9].cells.length;
for (var e9=0;e9<e8;e9++){
var i8=i3.rows[g9].cells[e9];
var j0=this.GetRender(i8);
if (j0!=null){
i6.style.fontFamily=j0.style.fontFamily;
i6.style.fontSize=j0.style.fontSize;
i6.style.fontWeight=j0.style.fontWeight;
i6.style.fontStyle=j0.style.fontStyle;
}
var m7=this.GetColFromCell(e4,i8);
i6.style.posWidth=this.GetColWidthFromCol(e4,m7);
if (j0!=null&&j0.tagName=="SELECT"){
var f7="";
for (var h9=0;h9<j0.childNodes.length;h9++){
var p6=j0.childNodes[h9];
if (p6.text!=null&&p6.text.length>f7.length)f7=p6.text;
}
i6.innerHTML=f7;
}
else if (j0!=null&&j0.tagName=="INPUT")
i6.innerHTML=j0.value;
else 
{
i6.innerHTML=i8.innerHTML;
}
p5=i6.offsetHeight;
if (p5>i7)i7=p5;
}
return Math.max(0,i7)+3;
}
this.SetRowHeight2=function (e4,g9,height){
if (height<1){
height=1;
}
g9=this.GetDisplayIndex(e4,g9);
var b5=null;
if (this.GetRowHeader(e4)!=null)b5=this.GetRowHeader(e4).rows[g9];
if (b5!=null){
b5.style.posHeight=height;
b5.cells[0].style.posHeight=height;
}
var i3=this.GetViewport(e4);
if (b5!=null){
i3.rows[b5.rowIndex].cells[0].style.posHeight=b5.style.posHeight;
}else if (i3!=null){
i3.rows[g9].cells[0].style.posHeight=height;
b5=i3.rows[g9];
}
var p7=this.AddRowInfo(e4,b5.FpKey);
if (p7!=null){
this.SetRowHeight(e4,p7,b5.style.posHeight);
}
var i4=this.GetParentSpread(e4);
if (i4!=null)i4.UpdateRowHeight(e4);
this.SizeSpread(e4);
}
this.GetRowHeightInternal=function (e4,g9){
var b5=null;
if (this.GetRowHeader(e4)!=null)
b5=this.GetRowHeader(e4).rows[g9];
else if (this.GetViewport(e4)!=null)
b5=this.GetViewport(e4).rows[g9];
if (b5!=null)
return b5.offsetHeight;
else 
return 0;
}
this.GetCell=function (ele,noHeader,event){
var f7=ele;
while (f7!=null){
if (noHeader){
if ((f7.tagName=="TD"||f7.tagName=="TH")&&(f7.parentNode.getAttribute("FpSpread")=="r")){
return f7;
}
}else {
if ((f7.tagName=="TD"||f7.tagName=="TH")&&(f7.parentNode.getAttribute("FpSpread")=="r"||f7.parentNode.getAttribute("FpSpread")=="ch"||f7.parentNode.getAttribute("FpSpread")=="rh")){
return f7;
}
}
f7=f7.parentNode;
}
return null;
}
this.InRowHeader=function (e4,h2){
return (this.IsChild(h2,this.GetRowHeader(e4)));
}
this.InColHeader=function (e4,h2){
return (this.IsChild(h2,this.GetColHeader(e4)));
}
this.InColFooter=function (e4,h2){
return (this.IsChild(h2,this.GetColFooter(e4)));
}
this.IsHeaderCell=function (e4,h2){
return (h2!=null&&(h2.tagName=="TD"||h2.tagName=="TH")&&(h2.parentNode.getAttribute("FpSpread")=="ch"||h2.parentNode.getAttribute("FpSpread")=="rh"));
}
this.GetSizeColumn=function (e4,ele,event){
if (ele.tagName!="TD"||(this.GetColHeader(e4)==null))return null;
var m7=-1;
var f7=ele;
var p3=this.GetViewport(this.GetTopSpread(e4)).parentNode.scrollLeft+window.scrollX;
while (f7!=null&&f7.parentNode!=null&&f7.parentNode!=document.documentElement){
if (f7.parentNode.getAttribute("FpSpread")=="ch"){
var p8=this.GetOffsetLeft(e4,f7);
var p9=p8+f7.offsetWidth;
if (event.clientX+p3<p8+3){
m7=this.GetColFromCell(e4,f7)-1;
}
else if (event.clientX+p3>p9-4){
m7=this.GetColFromCell(e4,f7);
var q0=this.GetSpanCell(f7.parentNode.rowIndex,m7,e4.e1);
if (q0!=null){
m7=q0.col+q0.colCount-1;
}
}else {
m7=this.GetColFromCell(e4,f7);
var q0=this.GetSpanCell(f7.parentNode.rowIndex,m7,e4.e1);
if (q0!=null){
var i9=p8;
m7=-1;
for (var e9=q0.col;e9<q0.col+q0.colCount&&e9<this.GetColCount(e4);e9++){
if (this.IsChild(f7,this.GetColHeader(e4)))
i9+=parseInt(this.GetElementById(this.GetColHeader(e4),e4.id+"col"+e9).width);
if (event.clientX>i9-3&&event.clientX<i9+3){
m7=e9;
break ;
}
}
}else {
m7=-1;
}
}
if (isNaN(m7)||m7<0)return null;
var q1=0;
var q2=this.GetColCount(e4);
var q3=true;
var e5=null;
var h1=m7+1;
while (h1<q2){
var m6=this.GetColGroup(this.GetColHeader(e4));
if (h1<m6.childNodes.length)
q1=parseInt(m6.childNodes[h1].width);
if (q1>1){
q3=false;
break ;
}
h1++;
}
if (q3){
h1=m7+1;
while (h1<q2){
if (this.GetSizable(e4,h1)){
m7=h1;
break ;
}
h1++;
}
}
if (!this.GetSizable(e4,m7))return null;
if (this.IsChild(f7,this.GetColHeader(e4))){
return this.GetElementById(this.GetColHeader(e4),e4.id+"col"+m7);
}
}
f7=f7.parentNode;
}
return null;
}
this.GetColGroup=function (f7){
if (f7==null)return null;
var m6=f7.getElementsByTagName("COLGROUP");
if (m6!=null&&m6.length>0){
if (f7.colgroup!=null)return f7.colgroup;
var j6=new Object();
j6.childNodes=new Array();
for (var e9=0;e9<m6[0].childNodes.length;e9++){
if (m6[0].childNodes[e9]!=null&&m6[0].childNodes[e9].tagName=="COL"){
var e8=j6.childNodes.length;
j6.childNodes.length++;
j6.childNodes[e8]=m6[0].childNodes[e9];
}
}
f7.colgroup=j6;
return j6;
}else {
return null;
}
}
this.GetSizeRow=function (e4,ele,event){
var n0=this.GetRowCount(e4);
if (n0==0)return null;
var h2=this.GetCell(ele);
if (h2==null){
if (ele.getAttribute("FpSpread")=="rowpadding"){
if (event.clientY<3){
var e8=ele.parentNode.rowIndex;
if (e8>1){
var i5=ele.parentNode.parentNode.rows[e8-1];
if (this.GetSizable(e4,i5))
return i5;
}
}
}
var c4=this.GetCorner(e4);
if (c4!=null&&this.IsChild(ele,c4)){
if (event.clientY>ele.offsetHeight-4){
var q4=null;
var e8=0;
q4=this.GetRowHeader(e4);
if (q4!=null){
while (e8<q4.rows.length&&q4.rows[e8].offsetHeight<2&&!this.GetSizable(e4,q4.rows[e8]))
e8++;
if (e8<q4.rows.length&&this.GetSizable(e4,q4.rows[e8])&&q4.rows[e8].offsetHeight<2)
return q4.rows[e8];
}
}else {
}
}
return null;
}
var e0=e4.e0;
var d9=e4.d9;
var f7=h2;
var p2=this.GetViewport(this.GetTopSpread(e4)).parentNode.scrollTop+window.scrollY;
while (f7!=null&&f7!=document.documentElement){
if (f7.getAttribute("FpSpread")=="rh"){
var e8=-1;
var q5=this.GetOffsetTop(e4,f7);
var q6=q5+f7.offsetHeight;
if (event.clientY+p2<q5+3){
if (f7.rowIndex>1)
e8=f7.rowIndex-1;
}
else if (event.clientY+p2>q6-4){
var q0=this.GetSpanCell(this.GetRowFromCell(e4,h2),this.GetColFromCell(e4,h2),e0);
if (q0!=null){
var j7=q5;
for (var e9=q0.row;e9<q0.row+q0.rowCount;e9++){
if (this.GetRowHeader(e4).rows[e9].cells.length>0)
j7+=parseInt(this.GetRowHeader(e4).rows[e9].cells[0].style.height);
if (event.clientY>j7-3&&event.clientY<j7+3){
e8=e9;
break ;
}
}
}else {
if (f7.rowIndex>=0)e8=f7.rowIndex;
}
}
else {
break ;
}
var j7=0;
var n0=this.GetRowHeader(e4).rows.length;
var q7=true;
var q4=null;
q4=this.GetRowHeader(e4);
var g9=e8+1;
while (g9<n0){
if (q4.rows[g9].style.height!=null)j7=parseInt(q4.rows[g9].style.height);
else j7=parseInt(q4.rows[g9].offsetHeight);
if (j7>1){
q7=false;
break ;
}
g9++;
}
if (q7){
g9=e8+1;
while (g9<n0){
if (this.GetSizable(e4,this.GetRowHeader(e4).rows[g9])){
e8=g9;
break ;
}
g9++;
}
}
if (e8>=0&&this.GetSizable(e4,q4.rows[e8])){
return q4.rows[e8];
}
else if (event.clientY<3){
while (e8>0&&q4.rows[e8].offsetHeight==0&&!this.GetSizable(e4,q4.rows[e8]))
e8--;
if (e8>=0&&this.GetSizable(e4,q4.rows[e8]))
return q4.rows[e8];
else 
return null;
}
}
f7=f7.parentNode;
}
return null;
}
this.GetElementById=function (i4,id){
if (i4==null)return null;
var f7=i4.firstChild;
while (f7!=null){
if (f7.id==id||(typeof(f7.getAttribute)=="function"&&f7.getAttribute("name")==id))return f7;
var j6=this.GetElementById(f7,id)
if (j6!=null)return j6;
f7=f7.nextSibling;
}
return null;
}
this.GetSizable=function (e4,ele){
if (typeof(ele)=="number"){
var h2=this.GetElementById(this.GetColHeader(e4),e4.id+"col"+ele);
return (h2!=null&&(h2.getAttribute("Sizable")==null||h2.getAttribute("Sizable")=="True"));
}
return (ele!=null&&(ele.getAttribute("Sizable")==null||ele.getAttribute("Sizable")=="True"));
}
this.GetSpanWidth=function (e4,m7,q2){
var i9=0;
var e5=this.GetViewport(e4);
if (e5!=null){
var m6=this.GetColGroup(e5);
if (m6!=null){
for (var e9=m7;e9<m7+q2;e9++){
i9+=parseInt(m6.childNodes[e9].width);
}
}
}
return i9;
}
this.GetCellType=function (h2){
if (h2!=null&&h2.getAttribute("FpCellType")!=null)return h2.getAttribute("FpCellType");
if (h2!=null&&h2.getAttribute("FpRef")!=null){
var f7=document.getElementById(h2.getAttribute("FpRef"));
return f7.getAttribute("FpCellType");
}
if (h2!=null&&h2.getAttribute("FpCellType")!=null)return h2.getAttribute("FpCellType");
return "text";
}
this.GetCellType2=function (h2){
if (h2!=null&&h2.getAttribute("FpRef")!=null){
h2=document.getElementById(h2.getAttribute("FpRef"));
}
var j1=null;
if (h2!=null){
j1=h2.getAttribute("FpCellType");
if (j1=="readonly")j1=h2.getAttribute("CellType");
if (j1==null&&h2.getAttribute("CellType2")=="TagCloudCellType")
j1=h2.getAttribute("CellType2");
}
if (j1!=null)return j1;
return "text";
}
this.GetCellEditorID=function (e4,h2){
if (h2!=null&&h2.getAttribute("FpRef")!=null){
var f7=document.getElementById(h2.getAttribute("FpRef"));
return f7.getAttribute("FpEditorID");
}
if (h2.getAttribute("FpEditorID")!=null)
return h2.getAttribute("FpEditorID");
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
for (var e9=0;e9<this.c0.length;e9++){
var q8=this.c0[e9];
if (q8.id==editorID){
a8=q8.a8;
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
this.GetCellValidatorID=function (e4,h2){
return null;
}
this.GetCellValidator=function (e4,validatorID){
return null;
}
this.GetTableRow=function (e4,g9){
var f4=this.GetData(e4).getElementsByTagName("root")[0];
var f3=f4.getElementsByTagName("data")[0];
var f7=f3.firstChild;
while (f7!=null){
if (f7.getAttribute("key")==""+g9)return f7;
f7=f7.nextSibling;
}
return null;
}
this.GetTableCell=function (i5,h1){
if (i5==null)return null;
var f7=i5.firstChild;
while (f7!=null){
if (f7.getAttribute("key")==""+h1)return f7;
f7=f7.nextSibling;
}
return null;
}
this.AddTableRow=function (e4,g9){
if (g9==null)return null;
var m9=this.GetTableRow(e4,g9);
if (m9!=null)return m9;
var f4=this.GetData(e4).getElementsByTagName("root")[0];
var f3=f4.getElementsByTagName("data")[0];
if (document.all!=null){
m9=this.GetData(e4).createNode("element","row","");
}else {
m9=document.createElement("row");
m9.style.display="none";
}
m9.setAttribute("key",g9);
f3.appendChild(m9);
return m9;
}
this.AddTableCell=function (i5,h1){
if (i5==null)return null;
var m9=this.GetTableCell(i5,h1);
if (m9!=null)return m9;
if (document.all!=null){
m9=this.GetData(e4).createNode("element","cell","");
}else {
m9=document.createElement("cell");
m9.style.display="none";
}
m9.setAttribute("key",h1);
i5.appendChild(m9);
return m9;
}
this.GetCellValue=function (e4,h2){
if (h2==null)return null;
var g9=this.GetRowKeyFromCell(e4,h2);
var h1=this.GetColKeyFromCell(e4,h2);
var q9=this.AddTableCell(this.AddTableRow(e4,g9),h1);
return q9.innerHTML;
}
this.HTMLEncode=function (s){
var r0=new String(s);
var r1=new RegExp("&","g");
r0=r0.replace(r1,"&amp;");
r1=new RegExp("<","g");
r0=r0.replace(r1,"&lt;");
r1=new RegExp(">","g");
r0=r0.replace(r1,"&gt;");
r1=new RegExp("\"","g");
r0=r0.replace(r1,"&quot;");
return r0;
}
this.HTMLDecode=function (s){
var r0=new String(s);
var r1=new RegExp("&amp;","g");
r0=r0.replace(r1,"&");
r1=new RegExp("&lt;","g");
r0=r0.replace(r1,"<");
r1=new RegExp("&gt;","g");
r0=r0.replace(r1,">");
r1=new RegExp("&quot;","g");
r0=r0.replace(r1,'"');
return r0;
}
this.SetCellValue=function (e4,h2,val,noEvent,recalc){
if (h2==null)return ;
var r2=this.GetCellType(h2);
if (r2=="readonly")return ;
var g9=this.GetRowKeyFromCell(e4,h2);
var h1=this.GetColKeyFromCell(e4,h2);
var q9=this.AddTableCell(this.AddTableRow(e4,g9),h1);
val=this.HTMLEncode(val);
val=this.HTMLEncode(val);
q9.innerHTML=val;
if (!noEvent){
var g0=this.CreateEvent("DataChanged");
g0.cell=h2;
g0.cellValue=val;
g0.row=g9;
g0.col=h1;
this.FireEvent(e4,g0);
}
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.GetSelectedRanges=function (e4){
var m8=this.GetSelection(e4);
var g2=new Array();
var m9=m8.firstChild;
while (m9!=null){
var h7=new this.Range();
this.GetRangeFromNode(e4,m9,h7);
var f7=g2.length;
g2.length=f7+1;
g2[f7]=h7;
m9=m9.nextSibling;
}
return g2;
}
this.GetSelectedRange=function (e4){
var h7=new this.Range();
var m8=this.GetSelection(e4);
var m9=m8.lastChild;
if (m9!=null){
this.GetRangeFromNode(e4,m9,h7);
}
return h7;
}
this.GetRangeFromNode=function (e4,m9,h7){
if (m9==null||e4==null||h7==null)return ;
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
var i3=this.GetViewport(e4);
if (i3!=null){
var r3=this.GetDisplayIndex(e4,g9);
for (var e9=r3;e9<r3+n0;e9++){
if (this.IsChildSpreadRow(e4,i3,e9))n0--;
}
}
var r4=null;
if (g9<0&&h1<0&&n0!=0&&g8!=0)
r4="Table";
else if (g9<0&&h1>=0&&g8>0)
r4="Column";
else if (h1<0&&g9>=0&&n0>0)
r4="Row";
else 
r4="Cell";
this.SetRange(h7,r4,g9,h1,n0,g8);
}
this.GetSelection=function (e4){
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var r5=n1.getElementsByTagName("selection")[0];
return r5;
}
this.GetRowKeyFromRow=function (e4,g9){
if (g9<0)return null;
var e5=null;
e5=this.GetViewport(e4);
return e5.rows[g9].getAttribute("FpKey");
}
this.GetColKeyFromCol=function (e4,h1){
if (h1<0)return null;
var e5=this.GetViewport(e4);
var m6=this.GetColGroup(e5);
if (m6==null||m6.childNodes.length==0)
m6=this.GetColGroup(this.GetColHeader(e4));
if (m6!=null&&h1>=0&&h1<m6.childNodes.length){
return m6.childNodes[h1].getAttribute("FpCol");
}
return null;
}
this.GetRowKeyFromCell=function (e4,h2){
var g9=h2.parentNode.getAttribute("FpKey");
return g9;
}
this.GetColKeyFromCell=function (e4,h2){
var m7=this.GetColFromCell(e4,h2);
var e5=this.GetViewport(e4);
var m6=this.GetColGroup(e5);
if (m6!=null&&m7>=0&&m7<m6.childNodes.length){
return m6.childNodes[m7].getAttribute("FpCol");
}
return null;
}
this.SetSelection=function (e4,i5,m7,rowcount,colcount,addSelection){
if (!e4.initialized)return ;
var r6=i5;
var r7=m7;
if (i5!=null&&parseInt(i5)>=0){
i5=this.GetRowKeyFromRow(e4,i5);
if (i5!="newRow")
i5=parseInt(i5);
}
if (m7!=null&&parseInt(m7)>=0){
m7=parseInt(this.GetColKeyFromCol(e4,m7));
}
var m9=this.GetSelection(e4);
if (m9==null)return ;
if (addSelection==null)
addSelection=(e4.getAttribute("multiRange")=="true"&&!this.a6);
var r8=m9.lastChild;
if (r8==null||addSelection){
if (document.all!=null){
r8=this.GetData(e4).createNode("element","range","");
}else {
r8=document.createElement('range');
r8.style.display="none";
}
m9.appendChild(r8);
}
r8.setAttribute("row",i5);
r8.setAttribute("col",m7);
r8.setAttribute("rowcount",rowcount);
r8.setAttribute("colcount",colcount);
r8.setAttribute("rowIndex",r6);
r8.setAttribute("colIndex",r7);
e4.e2=true;
this.PaintFocusRect(e4);
var f6=this.GetCmdBtn(e4,"Update");
this.UpdateCmdBtnState(f6,false);
var g0=this.CreateEvent("SelectionChanged");
this.FireEvent(e4,g0);
}
this.CreateSelectionNode=function (e4,i5,m7,rowcount,colcount,r6,r7){
var r8=document.createElement('range');
r8.style.display="none";
r8.setAttribute("row",i5);
r8.setAttribute("col",m7);
r8.setAttribute("rowcount",rowcount);
r8.setAttribute("colcount",colcount);
r8.setAttribute("rowIndex",r6);
r8.setAttribute("colIndex",r7);
return r8;
}
this.AddRowToSelection=function (e4,m9,i5){
var r6=i5;
if (typeof(i5)!="undefined"&&parseInt(i5)>=0){
i5=this.GetRowKeyFromRow(e4,i5);
if (i5!="newRow")
i5=parseInt(i5);
}
if (!this.IsRowSelected(e4,i5)&&!isNaN(i5))
{
var r8=this.CreateSelectionNode(e4,i5,-1,1,-1,r6,-1);
m9.appendChild(r8);
}
}
this.RemoveSelection=function (e4,i5,m7,rowcount,colcount){
var m9=this.GetSelection(e4);
if (m9==null)return ;
var r8=m9.firstChild;
while (r8!=null){
var g9=parseInt(r8.getAttribute("rowIndex"));
var n0=parseInt(r8.getAttribute("rowcount"));
if (g9<=i5&&i5<g9+n0){
m9.removeChild(r8);
for (var e9=g9;e9<g9+n0;e9++){
if (e9!=i5){
this.AddRowToSelection(e4,m9,e9);
}
}
break ;
}
r8=r8.nextSibling;
}
e4.e2=true;
var f6=this.GetCmdBtn(e4,"Update");
this.UpdateCmdBtnState(f6,false);
var g0=this.CreateEvent("SelectionChanged");
this.FireEvent(e4,g0);
}
this.GetColInfo=function (e4,h1){
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var m7=n1.getElementsByTagName("colinfo")[0];
var f7=m7.firstChild;
while (f7!=null){
if (f7.getAttribute("key")==""+h1)return f7;
f7=f7.nextSibling;
}
return null;
}
this.GetColWidthFromCol=function (e4,h1){
var m6=this.GetColGroup(this.GetViewport(e4));
return parseInt(m6.childNodes[h1].width);
}
this.GetColWidth=function (colInfo){
if (colInfo==null)return null;
var m9=colInfo.getElementsByTagName("width")[0];
if (m9!=null)return m9.innerHTML;
return 0;
}
this.AddColInfo=function (e4,h1){
var m9=this.GetColInfo(e4,h1);
if (m9!=null)return m9;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var m7=n1.getElementsByTagName("colinfo")[0];
if (document.all!=null){
m9=this.GetData(e4).createNode("element","col","");
}else {
m9=document.createElement('col');
m9.style.display="none";
}
m9.setAttribute("key",h1);
m7.appendChild(m9);
return m9;
}
this.SetColWidth=function (e4,m7,width){
if (m7==null)return ;
m7=parseInt(m7);
var j8=this.IsXHTML(e4);
var r9=null;
if (this.GetViewport(e4)!=null){
var m6=this.GetColGroup(this.GetViewport(e4));
if (m6==null||m6.childNodes.length==0){
m6=this.GetColGroup(this.GetColHeader(e4));
}
r9=this.AddColInfo(e4,m6.childNodes[m7].getAttribute("FpCol"));
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (m7==0)width-=1;
}
if (width==0)width=1;
if (m6!=null)
m6.childNodes[m7].width=width;
this.SetWidthFix(this.GetViewport(e4),m7,width);
}
if (this.GetColHeader(e4)!=null){
if (this.GetViewport(e4)!=null){
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (j8){
if (m7==this.colCount-1)width-=1;
}
}
}
if (width<=0)width=1;
document.getElementById(e4.id+"col"+m7).width=width;
this.SetWidthFix(this.GetColHeader(e4),m7,width);
if (this.GetViewport(e4)!=null){
if (this.GetViewport(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetViewport(e4).rules!="rows"){
if (m7==this.GetColCount(e4)-1)width+=1;
}
}
}
if (this.GetColFooter(e4)!=null){
var m6=this.GetColGroup(this.GetColFooter(e4));
if (m6==null||m6.childNodes.length==0){
m6=this.GetColGroup(this.GetColHeader(e4));
}
r9=this.AddColInfo(e4,m6.childNodes[m7].getAttribute("FpCol"));
if (this.GetColFooter(e4).cellSpacing=="0"&&this.GetColCount(e4)>1&&this.GetColFooter(e4).rules!="rows"){
if (m7==0)width-=1;
}
if (width==0)width=1;
if (m6!=null)
m6.childNodes[m7].width=width;
this.SetWidthFix(this.GetColFooter(e4),m7,width);
}
var e7=this.GetTopSpread(e4);
this.SizeAll(e7);
this.Refresh(e7);
if (r9!=null){
var m9=r9.getElementsByTagName("width");
if (m9!=null&&m9.length>0)
m9[0].innerHTML=width;
else {
if (document.all!=null){
m9=this.GetData(e4).createNode("element","width","");
}else {
m9=document.createElement('width');
m9.style.display="none";
}
r9.appendChild(m9);
m9.innerHTML=width;
}
}
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null)this.UpdateCmdBtnState(f6,false);
e4.e2=true;
}
this.SetWidthFix=function (e5,m7,width){
if (e5==null||e5.rows.length==0)return ;
var e9=0;
var s0=0;
var i8=e5.rows[0].cells[0];
var s1=i8.colSpan;
if (s1==null)s1=1;
while (m7>s0+s1){
e9++;
s0=s0+s1;
i8=e5.rows[0].cells[e9];
s1=i8.colSpan;
if (s1==null)s1=1;
}
i8.width=width;
}
this.GetRowInfo=function (e4,g9){
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var i5=n1.getElementsByTagName("rowinfo")[0];
var f7=i5.firstChild;
while (f7!=null){
if (f7.getAttribute("key")==""+g9)return f7;
f7=f7.nextSibling;
}
return null;
}
this.GetRowHeight=function (p7){
if (p7==null)return null;
var s2=p7.getElementsByTagName("height");
if (s2!=null&&s2.length>0)return s2[0].innerHTML;
return 0;
}
this.AddRowInfo=function (e4,g9){
var m9=this.GetRowInfo(e4,g9);
if (m9!=null)return m9;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var i5=n1.getElementsByTagName("rowinfo")[0];
if (document.all!=null){
m9=this.GetData(e4).createNode("element","row","");
}else {
m9=document.createElement('row');
m9.style.display="none";
}
m9.setAttribute("key",g9);
i5.appendChild(m9);
return m9;
}
this.GetTopSpread=function (g0)
{
if (g0==null)return null;
var g2=this.GetSpread(g0);
if (g2==null)return null;
var f7=g2.parentNode;
while (f7!=null&&f7.tagName!="BODY")
{
if (f7.getAttribute&&f7.getAttribute("FpSpread")=="Spread"){
if (f7.getAttribute("hierView")=="true")
g2=f7;
else 
break ;
}
f7=f7.parentNode;
}
return g2;
}
this.GetParentSpread=function (e4)
{
try {
var f7=e4.parentNode;
while (f7!=null&&f7.getAttribute&&f7.getAttribute("FpSpread")!="Spread")f7=f7.parentNode;
if (f7!=null&&f7.getAttribute&&f7.getAttribute("hierView")=="true")
return f7;
else 
return null;
}catch (g0){
return null;
}
}
this.SetRowHeight=function (e4,p7,height){
if (p7==null)return ;
var m9=p7.getElementsByTagName("height");
if (m9!=null&&m9.length>0)
m9[0].innerHTML=height;
else {
if (document.all!=null){
m9=this.GetData(e4).createNode("element","height","");
}else {
m9=document.createElement('height');
m9.style.display="none";
}
p7.appendChild(m9);
m9.innerHTML=height;
}
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null)this.UpdateCmdBtnState(f6,false);
e4.e2=true;
}
this.SetActiveRow=function (e4,i5){
if (this.GetRowCount(e4)<1)return ;
if (i5==null)i5=-1;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var n2=n1.firstChild;
while (n2!=null&&n2.tagName!="activerow"&&n2.tagName!="ACTIVEROW"){
n2=n2.nextSibling;
}
if (n2!=null)
n2.innerHTML=""+i5;
if (i5!=null&&e4.getAttribute("IsNewRow")!="true"&&e4.getAttribute("AllowInsert")=="true"){
var f6=this.GetCmdBtn(e4,"Insert");
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Add");
this.UpdateCmdBtnState(f6,false);
}else {
var f6=this.GetCmdBtn(e4,"Insert");
this.UpdateCmdBtnState(f6,true);
f6=this.GetCmdBtn(e4,"Add");
this.UpdateCmdBtnState(f6,true);
}
if (i5!=null&&e4.getAttribute("IsNewRow")!="true"&&(e4.getAttribute("AllowDelete")==null||e4.getAttribute("AllowDelete")=="true")){
var f6=this.GetCmdBtn(e4,"Delete");
this.UpdateCmdBtnState(f6,(i5==-1));
}else {
var f6=this.GetCmdBtn(e4,"Delete");
this.UpdateCmdBtnState(f6,true);
}
e4.e2=true;
}
this.SetActiveCol=function (e4,m7){
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var n1=f4.getElementsByTagName("state")[0];
var n3=n1.firstChild;
while (n3!=null&&n3.tagName!="activecolumn"&&n3.tagName!="ACTIVECOLUMN"){
n3=n3.nextSibling;
}
if (n3!=null)
n3.innerHTML=""+parseInt(m7);
e4.e2=true;
}
this.GetEditor=function (h2){
if (h2==null)return null;
var r2=this.GetCellType(h2);
if (r2=="readonly")return null;
var i2=h2.getElementsByTagName("DIV");
if (r2=="MultiColumnComboBoxCellType"){
if (i2!=null&&i2.length>0){
var f7=i2[0];
f7.type="div";
return f7;
}
}
var i2=h2.getElementsByTagName("INPUT");
if (i2!=null&&i2.length>0){
var f7=i2[0];
while (f7!=null&&f7.getAttribute&&f7.getAttribute("FpEditor")==null)
f7=f7.parentNode;
if (!f7.getAttribute)f7=null;
return f7;
}
i2=h2.getElementsByTagName("SELECT");
if (i2!=null&&i2.length>0){
var f7=i2[0];
return f7;
}
return null;
}
this.GetPageActiveSpread=function (){
var s3=document.documentElement.getAttribute("FpActiveSpread");
var f7=null;
if (s3!=null)f7=document.getElementById(s3);
return f7;
}
this.GetPageActiveSheetView=function (){
var s3=document.documentElement.getAttribute("FpActiveSheetView");
var f7=null;
if (s3!=null)f7=document.getElementById(s3);
return f7;
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
var e8=the_fpSpread.spreads.length;
for (var e9=0;e9<e8;e9++){
if (the_fpSpread.spreads[e9]!=null)the_fpSpread.SizeSpread(the_fpSpread.spreads[e9]);
}
}
this.DblClick=function (event){
var h2=this.GetCell(this.GetTarget(event),true,event);
var e4=this.GetSpread(h2);
if (h2!=null&&!this.IsHeaderCell(h2)&&this.GetOperationMode(e4)=="RowMode"&&this.GetEnableRowEditTemplate(e4)=="true"){
var s4=h2.getElementsByTagName("DIV");
if (s4!=null&&s4.length>0&&s4[0].id==e4.id+"_RowEditTemplateContainer")return ;
this.Edit(e4,this.GetRowKeyFromCell(e4,h2));
var f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null)
this.UpdateCmdBtnState(f6,false);
return ;
}
if (h2!=null&&!this.IsHeaderCell(h2)&&h2==e4.d1)this.StartEdit(e4,h2);
}
this.GetEvent=function (g0){
if (g0!=null)return g0;
return window.event;
}
this.GetTarget=function (g0){
g0=this.GetEvent(g0);
if (g0.target==document){
if (g0.currentTarget!=null)return g0.currentTarget;
}
if (g0.target!=null)return g0.target;
return g0.srcElement;
}
this.StartEdit=function (e4,editCell){
var s5=this.GetOperationMode(e4);
if (s5=="SingleSelect"||s5=="ReadOnly"||this.a7)return ;
if (s5=="RowMode"&&this.GetEnableRowEditTemplate(e4)=="true")return ;
var h2=editCell;
if (h2==null)h2=e4.d1;
if (h2==null)return ;
this.b1=-1;
var i2=this.GetEditor(h2);
if (i2!=null){
this.a7=true;
this.a8=i2;
this.b1=1;
}
var j8=this.IsXHTML(e4);
if (h2!=null){
var g9=this.GetRowFromCell(e4,h2);
var h1=this.GetColFromCell(e4,h2);
var g0=this.CreateEvent("EditStart");
g0.cell=h2;
g0.row=this.GetSheetIndex(e4,g9);
g0.col=h1;
g0.cancel=false;
this.FireEvent(e4,g0);
if (g0.cancel)return ;
var r2=this.GetCellType(h2);
if (r2=="readonly")return ;
if (e4.d1!=h2){
e4.d1=h2;
this.SetActiveRow(e4,this.GetRowKeyFromCell(e4,h2));
this.SetActiveCol(e4,this.GetColKeyFromCell(e4,h2));
}
if (i2==null){
var j0=this.GetRender(h2);
var s6=this.GetValueFromRender(e4,j0);
if (s6==" ")s6="";
this.a9=s6;
this.b0=this.GetFormulaFromCell(h2);
if (this.b0!=null)s6=this.b0;
try {
if (j0!=h2){
j0.style.display="none";
}
else {
j0.innerHTML="";
}
}catch (g0){
return ;
}
var s7=this.GetCellEditorID(e4,h2);
if (s7!=null&&s7.length>0){
this.a8=this.GetCellEditor(e4,s7,true);
if (!this.a8.getAttribute("MccbId")&&!this.a8.getAttribute("Extenders"))
this.a8.style.display="inline";
else 
this.a8.style.display="block";
this.a8.id=s7+"Editor";
}else {
this.a8=document.createElement("INPUT");
this.a8.type="text";
}
this.a8.style.fontFamily=j0.style.fontFamily;
this.a8.style.fontSize=j0.style.fontSize;
this.a8.style.fontWeight=j0.style.fontWeight;
this.a8.style.fontStyle=j0.style.fontStyle;
this.a8.style.textDecoration=j0.style.textDecoration;
this.a8.style.position="";
if (j8){
var s8=h2.clientWidth-2;
var s9=parseInt(h2.style.paddingLeft);
if (!isNaN(s9))
s8-=s9;
s9=parseInt(h2.style.paddingRight);
if (!isNaN(s9))
s8-=s9;
this.a8.style.width=""+s8+"px";
}
else 
this.a8.style.width=h2.clientWidth-2;
this.SaveMargin(h2);
if (this.a8.tagName=="TEXTAREA")
this.a8.style.height=""+(h2.offsetHeight-4)+"px";
if ((this.a8.tagName=="INPUT"&&this.a8.type=="text")||this.a8.tagName=="TEXTAREA"){
if (this.a8.style.backgroundColor==""||this.a8.backColorSet!=null){
var t0="";
if (document.defaultView!=null&&document.defaultView.getComputedStyle!=null)t0=document.defaultView.getComputedStyle(h2,'').getPropertyValue("background-color");
if (t0!="")
this.a8.style.backgroundColor=t0;
else 
this.a8.style.backgroundColor=h2.bgColor;
this.a8.backColorSet=true;
}
if (this.a8.style.color==""||this.a8.colorSet!=null){
var t1="";
if (document.defaultView!=null&&document.defaultView.getComputedStyle!=null)t1=document.defaultView.getComputedStyle(h2,'').getPropertyValue("color");
this.a8.style.color=t1;
this.a8.colorSet=true;
}
this.a8.style.borderWidth="0px";
this.RestoreMargin(this.a8,false);
}
this.b1=0;
h2.insertBefore(this.a8,h2.firstChild);
this.SetEditorValue(this.a8,s6,e4);
if (this.a8.offsetHeight<h2.clientHeight&&this.a8.tagName!="TEXTAREA"){
if (h2.vAlign=="middle")
this.a8.style.posTop+=(h2.clientHeight-this.a8.offsetHeight)/2;
else if (h2.vAlign=="bottom")
this.a8.style.posTop+=(h2.clientHeight-this.a8.offsetHeight);
}
this.SizeAll(this.GetTopSpread(e4));
}
this.SetEditorFocus(this.a8);
if (e4.getAttribute("EditMode")=="replace"){
if ((this.a8.tagName=="INPUT"&&this.a8.type=="text")||this.a8.tagName=="TEXTAREA")
this.a8.select();
}
this.a7=true;
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null&&f6.disabled)
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Copy");
if (f6!=null&&!f6.disabled)
this.UpdateCmdBtnState(f6,true);
f6=this.GetCmdBtn(e4,"Paste");
if (f6!=null&&!f6.disabled)
this.UpdateCmdBtnState(f6,true);
f6=this.GetCmdBtn(e4,"Clear");
if (f6!=null&&!f6.disabled)
this.UpdateCmdBtnState(f6,true);
}
this.ScrollView(e4);
}
this.GetCurrency=function (validator){
var t2=validator.CurrencySymbol;
if (t2!=null)return t2;
var f7=document.getElementById(validator.id+"cs");
if (f7!=null){
return f7.innerText;
}
return "";
}
this.GetValueFromRender=function (e4,rd){
var j1=this.GetCellType2(this.GetCell(rd));
if (j1!=null){
if (j1=="text")j1="TextCellType";
var i1=null;
if (j1=="ExtenderCellType"){
i1=this.GetFunction(j1+"_getEditor")
if (i1!=null){
if (i1(rd)!=null)
i1=this.GetFunction(j1+"_getEditorValue");
else 
i1=null;
}
}else 
i1=this.GetFunction(j1+"_getValue");
if (i1!=null){
return i1(rd,e4);
}
}
var f7=rd;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
if (f7.innerHTML=="&nbsp;")return "";
var s6=f7.value;
if ((typeof(s6)=="undefined")&&j1=="readonly"&&f7.parentNode!=null&&f7.parentNode.getAttribute("CellType2")=="TagCloudCellType")
s6=f7.textContent;
if (s6==null){
s6=this.ReplaceAll(f7.innerHTML,"&nbsp;"," ");
s6=this.ReplaceAll(s6,"<br>"," ");
s6=this.HTMLDecode(s6);
}
return s6;
}
this.ReplaceAll=function (val,src,dest){
if (val==null)return val;
var t3=val.length;
while (true){
val=val.replace(src,dest);
if (val.length==t3)break ;
t3=val.length;
}
return val;
}
this.GetFormula=function (e4,g9,h1){
g9=this.GetDisplayIndex(e4,g9);
var h2=this.GetCellFromRowCol(e4,g9,h1);
var t4=this.GetFormulaFromCell(h2);
return t4;
}
this.SetFormula=function (e4,g9,h1,i1,recalc,clientOnly){
g9=this.GetDisplayIndex(e4,g9);
var h2=this.GetCellFromRowCol(e4,g9,h1);
h2.setAttribute("FpFormula",i1);
if (!clientOnly)
this.SetCellValue(e4,h2,i1,null,recalc);
}
this.GetFormulaFromCell=function (rd){
var s6=null;
if (rd.getAttribute("FpFormula")!=null){
s6=rd.getAttribute("FpFormula");
}
if (s6!=null)
s6=this.Trim(new String(s6));
return s6;
}
this.IsDouble=function (val,decimalchar,negsign,possign,minimumvalue,maximumvalue){
if (val==null||val.length==0)return true;
val=val.replace(" ","");
if (val.length==0)return true;
if (negsign!=null)val=val.replace(negsign,"-");
if (possign!=null)val=val.replace(possign,"+");
if (val.charAt(val.length-1)=="-")val="-"+val.substring(0,val.length-1);
var t5=new RegExp("^\\s*([-\\+])?(\\d+)?(\\"+decimalchar+"(\\d+))?([eE]([-\\+])?(\\d+))?\\s*$");
var t6=val.match(t5);
if (t6==null)
return false;
if ((t6[2]==null||t6[2].length==0)&&(t6[4]==null||t6[4].length==0))return false;
var t7="";
if (t6[1]!=null&&t6[1].length>0)t7=t6[1];
if (t6[2]!=null&&t6[2].length>0)
t7+=t6[2];
else 
t7+="0";
if (t6[4]!=null&&t6[4].length>0)
t7+=("."+t6[4]);
if (t6[6]!=null&&t6[6].length>0){
t7+=("E"+t6[6]);
if (t6[7]!=null)
t7+=(t6[7]);
else 
t7+="0";
}
var t8=parseFloat(t7);
if (isNaN(t8))return false;
var f7=true;
if (minimumvalue!=null){
var t9=parseFloat(minimumvalue);
f7=(!isNaN(t9)&&t8>=t9);
}
if (f7&&maximumvalue!=null){
var i7=parseFloat(maximumvalue);
f7=(!isNaN(i7)&&t8<=i7);
}
return f7;
}
this.GetFunction=function (fn){
if (fn==null||fn=="")return null;
try {
var f7=eval(fn);
return f7;
}catch (g0){
return null;
}
}
this.SetValueToRender=function (rd,val,valueonly){
var i1=null;
var j1=this.GetCellType2(this.GetCell(rd));
if (j1!=null){
if (j1=="text")j1="TextCellType";
if (j1=="ExtenderCellType"){
i1=this.GetFunction(j1+"_getEditor")
if (i1!=null){
if (i1(rd)!=null)
i1=this.GetFunction(j1+"_setEditorValue");
else 
i1=null;
}
}else 
i1=this.GetFunction(j1+"_setValue");
}
if (i1!=null){
i1(rd,val);
}else {
if (typeof(rd.value)!="undefined"){
if (val==null)val="";
rd.value=val;
}else {
var f7=rd;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
f7.innerHTML=this.ReplaceAll(val," ","&nbsp;");
}
}
if ((valueonly==null||!valueonly)&&rd.getAttribute("FpFormula")!=null){
rd.setAttribute("FpFormula",val);
}
}
this.Trim=function (r3){
var t6=r3.match(new RegExp("^\\s*(\\S+(\\s+\\S+)*)\\s*$"));
return (t6==null)?"":t6[1];
}
this.GetOffsetLeft=function (e4,h2,i4){
var e5=i4;
if (e5==null)e5=this.GetViewportFromCell(e4,h2);
var p8=0;
var f7=h2;
while (f7!=null&&f7!=e5){
p8+=f7.offsetLeft;
f7=f7.offsetParent;
}
return p8;
}
this.GetOffsetTop=function (e4,h2,i4){
var e5=i4;
if (e5==null)e5=this.GetViewportFromCell(e4,h2);
var u0=0;
var f7=h2;
while (f7!=null&&f7!=e5){
u0+=f7.offsetTop;
f7=f7.offsetParent;
}
return u0;
}
this.SetEditorFocus=function (f7){
if (f7==null)return ;
var u1=true;
var h2=this.GetCell(f7,true);
var j1=this.GetCellType(h2);
if (j1!=null){
var i1=this.GetFunction(j1+"_setFocus");
if (i1!=null){
i1(f7);
u1=false;
}
}
if (u1){
try {
f7.focus();
}catch (g0){}
}
}
this.SetEditorValue=function (f7,val,e4){
var h2=this.GetCell(f7,true);
var j1=this.GetCellType(h2);
if (j1!=null){
var i1=this.GetFunction(j1+"_setEditorValue");
if (i1!=null){
i1(f7,val,e4);
return ;
}
}
j1=f7.getAttribute("FpEditor");
if (j1!=null){
var i1=this.GetFunction(j1+"_setEditorValue");
if (i1!=null){
i1(f7,val,e4);
return ;
}
}
f7.value=val;
}
this.GetEditorValue=function (f7){
var h2=this.GetCell(f7,true);
var j1=this.GetCellType(h2);
if (j1!=null){
var i1=this.GetFunction(j1+"_getEditorValue");
if (i1!=null){
return i1(f7);
}
}
j1=f7.getAttribute("FpEditor");
if (j1!=null){
var i1=this.GetFunction(j1+"_getEditorValue");
if (i1!=null){
return i1(f7);
}
}
if (f7.type=="checkbox"){
if (f7.checked)
return "True";
else 
return "False";
}
else 
{
return f7.value;
}
}
this.CreateMsg=function (){
if (this.b2!=null)return ;
var f7=this.b2=document.createElement("div");
f7.style.position="absolute";
f7.style.background="yellow";
f7.style.color="red";
f7.style.border="1px solid black";
f7.style.display="none";
f7.style.width="120px";
}
this.SetMsg=function (msg){
this.CreateMsg();
this.b2.innerHTML=msg;
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
var h2=this.GetCell(this.a8.parentNode);
var e4=this.GetSpread(h2,false);
if (e4==null)return true;
var u2=this.GetEditorValue(this.a8);
var u3=u2;
if (typeof(u2)=="string")u3=this.Trim(u2);
var u4=(e4.getAttribute("AcceptFormula")=="true"&&u3!=null&&u3.charAt(0)=='=');
var i2=(this.b1!=0);
if (!u4&&!i2){
var u5=null;
var j1=this.GetCellType(h2);
if (j1!=null){
var i1=this.GetFunction(j1+"_isValid");
if (i1!=null){
u5=i1(h2,u2);
}
}
if (u5!=null&&u5!=""){
this.SetMsg(u5);
this.GetViewport(e4).parentNode.insertBefore(this.b2,this.GetViewport(e4).parentNode.firstChild);
this.ShowMsg(true);
this.SetValidatorPos(e4);
this.a8.focus();
return false;
}else {
this.ShowMsg(false);
}
}
if (!i2){
h2.removeChild(this.a8);
var u6=this.GetRender(h2);
if (u6.style.display=="none")u6.style.display="block";
if (this.b0!=null&&this.b0==u2){
this.SetValueToRender(u6,this.a9,true);
}else {
this.SetValueToRender(u6,u2);
}
this.RestoreMargin(h2);
}
if ((this.b0!=null&&this.b0!=u2)||(this.b0==null&&this.a9!=u2)){
this.SetCellValue(e4,h2,u2);
if (u2!=null&&u2.length>0&&u2.indexOf("=")==0)h2.setAttribute("FpFormula",u2);
}
if (!i2)
this.SizeAll(this.GetTopSpread(e4));
this.a8=null;
this.a7=false;
var g0=this.CreateEvent("EditStopped");
g0.cell=h2;
this.FireEvent(e4,g0);
this.Focus(e4);
var u7=e4.getAttribute("autoCalc");
if (u7!="false"){
if ((this.b0!=null&&this.b0!=u2)||(this.b0==null&&this.a9!=u2)){
this.UpdateValues(e4);
}
}
}
this.b1=-1;
return true;
}
this.SetValidatorPos=function (e4){
if (this.a8==null)return ;
var h2=this.GetCell(this.a8.parentNode);
if (h2==null)return ;
var f7=this.b2;
if (f7!=null&&f7.style.display!="none"){
if (f7!=null){
f7.style.left=""+(h2.offsetLeft)+"px";
f7.style.top=""+(h2.offsetTop+h2.offsetHeight)+"px";
}
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
this.RestoreMargin=function (h2,reset){
if (this.b3.left!=null&&this.b3.left!=-1){
h2.style.paddingLeft=this.b3.left;
if (reset==null||reset)this.b3.left=-1;
}
if (this.b3.right!=null&&this.b3.right!=-1){
h2.style.paddingRight=this.b3.right;
if (reset==null||reset)this.b3.right=-1;
}
if (this.b3.top!=null&&this.b3.top!=-1){
h2.style.paddingTop=this.b3.top;
if (reset==null||reset)this.b3.top=-1;
}
if (this.b3.bottom!=null&&this.b3.bottom!=-1){
h2.style.paddingBottom=this.b3.bottom;
if (reset==null||reset)this.b3.bottom=-1;
}
}
this.PaintSelectedCell=function (e4,h2,select,anchor){
if (h2==null)return ;
var u8=anchor?e4.getAttribute("anchorBackColor"):e4.getAttribute("selectedBackColor");
if (select){
if (h2.getAttribute("bgColorBak")==null)
h2.setAttribute("bgColorBak",document.defaultView.getComputedStyle(h2,"").getPropertyValue("background-color"));
if (h2.bgColor1==null)
h2.bgColor1=h2.style.backgroundColor;
h2.style.backgroundColor=u8;
if (h2.getAttribute("bgSelImg"))
h2.style.backgroundImage=h2.getAttribute("bgSelImg");
}else {
if (h2.bgColor1!=null)
h2.style.backgroundColor="";
if (h2.bgColor1!=null&&h2.bgColor1!="")
h2.style.backgroundColor=h2.bgColor1;
h2.style.backgroundImage="";
if (h2.getAttribute("bgImg")!=null)
h2.style.backgroundImage=h2.getAttribute("bgImg");
}
}
this.PaintAnchorCell=function (e4){
var u9=this.GetOperationMode(e4);
if (e4.d1==null||(u9!="Normal"&&u9!="RowMode"))return ;
if (u9=="MultiSelect"||u9=="ExtendedSelect")return ;
if (!this.IsChild(e4.d1,e4))return ;
var v0=(this.GetTopSpread(e4).getAttribute("hierView")!="true");
if (e4.getAttribute("showFocusRect")=="false")v0=false;
if (v0){
this.PaintSelectedCell(e4,e4.d1,false);
this.PaintFocusRect(e4);
this.PaintAnchorCellHeader(e4,true);
return ;
}
var f7=e4.d1.parentNode.cells[0].firstChild;
if (f7!=null&&f7.nodeName!="#text"&&f7.getAttribute("FpSpread")=="Spread")return ;
this.PaintSelectedCell(e4,e4.d1,true,true);
this.PaintAnchorCellHeader(e4,true);
}
this.ClearSelection=function (e4,thisonly){
var v1=this.GetParentSpread(e4);
if (thisonly==null&&v1!=null&&v1.getAttribute("hierView")=="true"){
this.ClearSelection(v1);
return ;
}
var i3=this.GetViewport(e4);
var g6=this.GetRowCount(e4);
if (i3!=null&&i3.rows.length>g6){
for (var e9=0;e9<i3.rows.length;e9++){
if (i3.rows[e9].cells.length>0&&i3.rows[e9].cells[0]!=null&&i3.rows[e9].cells[0].firstChild!=null&&i3.rows[e9].cells[0].firstChild.nodeName!="#text"){
var f7=i3.rows[e9].cells[0].firstChild;
if (f7.getAttribute("FpSpread")=="Spread"){
this.ClearSelection(f7,true);
}
}
}
}
this.DoclearSelection(e4);
if (e4.d1!=null){
var s5=this.GetOperationMode(e4);
if (s5=="RowMode"||s5=="SingleSelect"||s5=="ExtendedSelect"||s5=="MultiSelect"){
var h3=this.GetRowFromCell(e4,e4.d1);
this.PaintSelection(e4,h3,-1,1,-1,false);
}
this.PaintSelectedCell(e4,e4.d1,false);
this.PaintAnchorCellHeader(e4,false);
}else {
var h2=this.GetCellFromRowCol(e4,1,0);
if (h2!=null)this.PaintSelectedCell(e4,h2,false);
}
this.PaintFocusRect(e4);
e4.selectedCols=[];
e4.e2=true;
}
this.SetSelectedRange=function (e4,g9,h1,n0,g8){
this.ClearSelection(e4);
var g9=this.GetDisplayIndex(e4,g9);
var v2=0;
var v3=n0;
var i3=this.GetViewport(e4);
if (i3!=null){
for (e9=g9;e9<i3.rows.length&&v2<v3;e9++){
if (this.IsChildSpreadRow(e4,i3,e9)){;
n0++;
}else {
v2++;
}
}
}
this.PaintSelection(e4,g9,h1,n0,g8,true);
this.SetSelection(e4,g9,h1,n0,g8);
}
this.AddSelection=function (e4,g9,h1,n0,g8){
var g9=this.GetDisplayIndex(e4,g9);
var v2=0;
var v3=n0;
var i3=this.GetViewport(e4);
if (i3!=null){
for (e9=g9;e9<i3.rows.length&&v2<v3;e9++){
if (this.IsChildSpreadRow(e4,i3,e9)){;
n0++;
}else {
v2++;
}
}
}
this.PaintSelection(e4,g9,h1,n0,g8,true);
this.SetSelection(e4,g9,h1,n0,g8,true);
}
this.SelectRow=function (e4,index,v2,select,ignoreAnchor){
e4.d5=index;
e4.d6=-1;
if (!ignoreAnchor)this.UpdateAnchorCell(e4,index,0,false);
e4.d7="r";
this.PaintSelection(e4,index,-1,v2,-1,select);
if (select)
{
this.SetSelection(e4,index,-1,v2,-1);
}else {
this.RemoveSelection(e4,index,-1,v2,-1);
this.PaintFocusRect(e4);
}
}
this.SelectColumn=function (e4,index,v2,select,ignoreAnchor){
e4.d5=-1;
e4.d6=index;
if (!ignoreAnchor)this.UpdateAnchorCell(e4,0,index,false);
e4.d7="c";
this.PaintSelection(e4,-1,index,-1,v2,select);
if (select)
{
this.SetSelection(e4,-1,index,-1,v2);
this.AddColSelection(e4,index);
}
}
this.AddColSelection=function (e4,index){
var v4=0;
for (var e9=0;e9<e4.selectedCols.length;e9++){
if (e4.selectedCols[e9]==index)return ;
if (index>e4.selectedCols[e9])v4=e9+1;
}
e4.selectedCols.length++;
for (var e9=e4.selectedCols.length-1;e9>v4;e9--)
e4.selectedCols[e9]=e4.selectedCols[e9-1];
e4.selectedCols[v4]=index;
}
this.IsColSelected=function (e4,r7){
for (var e9=0;e9<e4.selectedCols.length;e9++)
if (e4.selectedCols[e9]==r7)return true;
return false;
}
this.SyncColSelection=function (e4){
e4.selectedCols=[];
var v5=this.GetSelectedRanges(e4);
for (var e9=0;e9<v5.length;e9++){
var h7=v5[e9];
if (h7.type=="Column"){
for (var h9=h7.col;h9<h7.col+h7.colCount;h9++){
this.AddColSelection(e4,h9);
}
}
}
}
this.InitMovingCol=function (e4,r7,isGroupBar,n7){
if (e4.getAttribute("LayoutMode")&&r7==-1)return ;
if (this.GetOperationMode(e4)!="Normal"){
e4.selectedCols=[];
e4.selectedCols.push(r7);
}
if (isGroupBar){
this.dragCol=r7;
this.dragViewCol=this.GetColByKey(e4,r7);
}else {
this.dragCol=parseInt(this.GetSheetColIndex(e4,r7));
this.dragViewCol=r7;
}
var v6=this.GetMovingCol(e4);
if (isGroupBar){
this.ClearSelection(e4);
v6.innerHTML="";
var v7=document.createElement("DIV");
v7.innerHTML=n7.innerHTML;
v7.style.borderTop="0px solid";
v7.style.borderLeft="0px solid";
v7.style.borderRight="#808080 1px solid";
v7.style.borderBottom="#808080 1px solid";
v7.style.width=""+Math.max(this.GetPreferredCellWidth(e4,n7),80)+"px";
v6.appendChild(v7);
if (e4.getAttribute("DragColumnCssClass")==null){
v6.style.backgroundColor=n7.style.backgroundColor;
v6.style.paddingTop="1px";
v6.style.paddingBottom="1px";
}
v6.style.top="-50px";
v6.style.left="-100px";
}else {
var v8=0;
v6.style.top="0px";
v6.style.left="-1000px";
v6.style.display="";
v6.innerHTML="";
var v9=document.createElement("TABLE");
v6.appendChild(v9);
var i5=document.createElement("TR");
v9.appendChild(i5);
for (var e9=0;e9<e4.selectedCols.length;e9++){
var h2=document.createElement("TD");
i5.appendChild(h2);
var w0;
var w1;
if (e4.getAttribute("columnHeaderAutoTextIndex")!=null)
w0=parseInt(e4.getAttribute("columnHeaderAutoTextIndex"));
else 
w0=e4.getAttribute("ColHeaders")-1;
w1=e4.selectedCols[e9];
var w2=this.GetHeaderCellFromRowCol(e4,w0,w1,true);
if (w2.getAttribute("FpCellType")=="ExtenderCellType"&&w2.getElementsByTagName("DIV").length>0){
var w3=this.GetEditor(w2);
var w4=this.GetFunction("ExtenderCellType_getEditorValue");
if (w3!==null&&w4!==null){
h2.innerHTML=w4(w3);
}
}
else 
h2.innerHTML=w2.innerHTML;
h2.style.cssText=w2.style.cssText;
h2.style.borderTop="0px solid";
h2.style.borderLeft="0px solid";
h2.style.borderRight="#808080 1px solid";
h2.style.borderBottom="#808080 1px solid";
h2.style.whiteSpace="nowrap";
h2.setAttribute("align","center");
var i9=Math.max(this.GetPreferredCellWidth(e4,w2),80);
h2.style.width=""+i9+"px";
v8+=i9;
}
if (e4.getAttribute("DragColumnCssClass")==null){
v6.style.backgroundColor=e4.getAttribute("SelectedBackColor");
v6.style.tableLayout="fixed";
v6.style.width=""+v8+"px";
}
}
e4.selectedCols.context=[];
var w5=e4.selectedCols.context;
var p8=0;
m6=this.GetColGroup(this.GetColHeader(e4));
if (m6!=null){
for (var e9=0;e9<m6.childNodes.length;e9++){
var w6=m6.childNodes[e9].offsetWidth;
w5.push({left:p8,width:w6});
p8+=w6;
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
this.GetSpanCell=function (g9,h1,span){
if (span==null){
return null;
}
var v2=span.length;
for (var e9=0;e9<v2;e9++){
var q0=span[e9];
var w7=(q0.row<=g9&&g9<q0.row+q0.rowCount&&q0.col<=h1&&h1<q0.col+q0.colCount);
if (w7)return q0;
}
return null;
}
this.IsCovered=function (e4,g9,h1,span){
var q0=this.GetSpanCell(g9,h1,span);
if (q0==null){
return false;
}else {
if (q0.row==g9&&q0.col==h1)return false;
return true;
}
}
this.IsSpanCell=function (e4,g9,h1){
var d9=e4.d9;
var v2=d9.length;
for (var e9=0;e9<v2;e9++){
var q0=d9[e9];
var w7=(q0.row==g9&&q0.col==h1);
if (w7)return q0;
}
return null;
}
this.SelectRange=function (e4,g9,h1,n0,g8,select){
e4.d7="";
this.UpdateRangeSelection(e4,g9,h1,n0,g8,select);
if (select){
this.SetSelection(e4,g9,h1,n0,g8);
this.PaintAnchorCell(e4);
}
}
this.UpdateRangeSelection=function (e4,g9,h1,n0,g8,select){
var i3=this.GetViewport(e4);
this.UpdateRangeSelection(e4,g9,h1,n0,g8,select,i3);
}
this.GetSpanCells=function (e4,i3){
if (i3==this.GetViewport(e4))
return e4.d9;
else if (i3==this.GetColHeader(e4))
return e4.e1;
else if (i3==this.GetColFooter(e4))
return e4.footerSpanCells;
else if (i3==this.GetRowHeader(e4))
return e4.e0;
return null;
}
this.UpdateRangeSelection=function (e4,g9,h1,n0,g8,select,i3){
if (i3==null)return ;
for (var e9=g9;e9<g9+n0&&e9<i3.rows.length;e9++){
if (this.IsChildSpreadRow(e4,i3,e9))continue ;
var w8=this.GetCellIndex(e4,e9,h1,this.GetSpanCells(e4,i3));
for (var h9=0;h9<g8;h9++){
if (this.IsCovered(e4,e9,h1+h9,this.GetSpanCells(e4,i3)))continue ;
if (w8<i3.rows[e9].cells.length){
this.PaintSelectedCell(e4,i3.rows[e9].cells[w8],select);
}
w8++;
}
}
}
this.GetColFromCell=function (e4,h2){
if (h2==null)return -1;
var g9=this.GetRowFromCell(e4,h2);
return this.GetColIndex(e4,g9,h2.cellIndex,this.GetSpanCells(e4,h2.parentNode.parentNode.parentNode),false,this.IsChild(h2,this.GetRowHeader(e4)));
}
this.GetRowFromCell=function (e4,h2){
if (h2==null||h2.parentNode==null)return -1;
var g9=h2.parentNode.rowIndex;
return g9;
}
this.GetColIndex=function (e4,e9,cellIndex,span,frozArea,c5){
var w9=false;
var e5=this.GetViewport(e4);
if (e5!=null)w9=e5.parentNode.getAttribute("hiddenCells");
if (w9&&span==e4.d9){
return cellIndex;
}
var x0=0;
var v2=this.GetColCount(e4);
var x1=0;
if (c5){
x1=0;
var m6=null;
if (this.GetRowHeader(e4)!=null)
m6=this.GetColGroup(this.GetRowHeader(e4));
if (m6!=null)
v2=m6.childNodes.length;
}
for (var h9=x1;h9<v2;h9++){
if (this.IsCovered(e4,e9,h9,span))continue ;
if (x0==cellIndex){
return h9;
}
x0++;
}
return v2;
}
this.GetCellIndex=function (e4,e9,r7,span){
var w9=false;
var e5=this.GetViewport(e4);
if (e5!=null)w9=e5.parentNode.getAttribute("hiddenCells");
if (w9&&span==e4.d9){
return r7;
}else {
var x1=0;
var v2=r7;
var x0=0;
for (var h9=0;h9<v2;h9++){
if (this.IsCovered(e4,e9,x1+h9,span))continue ;
x0++;
}
return x0;
}
}
this.NextCell=function (e4,event,key){
if (event.altKey)return ;
var x2=this.GetParent(this.GetViewport(e4));
if (e4.d1==null){
var i1=this.FireActiveCellChangingEvent(e4,0,0);
if (!i1){
e4.SetActiveCell(0,0);
var g0=this.CreateEvent("ActiveCellChanged");
g0.cmdID=e4.id;
g0.row=g0.Row=0;
g0.col=g0.Col=0;
this.FireEvent(e4,g0);
}
return ;
}
if (event.shiftKey&&key!=this.tab){
var p6=this.GetOperationMode(e4);
if (p6=="RowMode"||p6=="SingleSelect"||p6=="MultiSelect"||(p6=="Normal"&&this.GetSelectionPolicy(e4)=="Single"))return ;
var q0=this.GetSpanCell(e4.d3,e4.d4,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
switch (key){
case this.right:
var g9=e4.d3;
var h1=e4.d4+1;
if (q0!=null){
h1=q0.col+q0.colCount;
}
if (h1>this.GetColCount(e4)-1)return ;
e4.d4=h1;
e4.d2=this.GetCellFromRowCol(e4,g9,h1);
this.Select(e4,e4.d1,e4.d2);
break ;
case this.left:
var g9=e4.d3;
var h1=e4.d4-1;
if (q0!=null){
h1=q0.col-1;
}
q0=this.GetSpanCell(g9,h1,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (q0!=null){
if (this.IsSpanCell(e4,g9,q0.col))h1=q0.col;
}
if (h1<0)return ;
e4.d4=h1;
e4.d2=this.GetCellFromRowCol(e4,g9,h1);
this.Select(e4,e4.d1,e4.d2);
break ;
case this.down:
var g9=e4.d3+1;
var h1=e4.d4;
if (q0!=null){
g9=q0.row+q0.rowCount;
}
g9=this.GetNextRow(e4,g9);
if (g9>this.GetRowCountInternal(e4)-1)return ;
e4.d3=g9;
e4.d2=this.GetCellFromRowCol(e4,g9,h1);
this.Select(e4,e4.d1,e4.d2);
break ;
case this.up:
var g9=e4.d3-1;
var h1=e4.d4;
if (q0!=null){
g9=q0.row-1;
}
g9=this.GetPrevRow(e4,g9);
q0=this.GetSpanCell(g9,h1,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (q0!=null){
if (this.IsSpanCell(e4,q0.row,h1))g9=q0.row;
}
if (g9<0)return ;
e4.d3=g9;
e4.d2=this.GetCellFromRowCol(e4,g9,h1);
this.Select(e4,e4.d1,e4.d2);
break ;
case this.home:
if (e4.d1.parentNode.rowIndex>=0){
e4.d4=0;
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
break ;
case this.end:
if (e4.d1.parentNode.rowIndex>=0){
e4.d4=this.GetColCount(e4)-1;
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
break ;
case this.pdn:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>=0){
g9=0;
for (g9=0;g9<this.GetViewport(e4).rows.length;g9++){
if (this.GetViewport(e4).rows[g9].offsetTop+this.GetViewport(e4).rows[g9].offsetHeight>this.GetViewport(e4).parentNode.offsetHeight+this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
g9=this.GetNextRow(e4,g9);
if (g9<this.GetViewport(e4).rows.length){
this.GetViewport(e4).parentNode.scrollTop=this.GetViewport(e4).rows[g9].offsetTop;
e4.d3=g9;
}else {
g9=this.GetRowCountInternal(e4)-1;
e4.d3=g9;
}
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
break ;
case this.pup:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>0){
g9=0;
for (g9=0;g9<this.GetViewport(e4).rows.length;g9++){
if (this.GetViewport(e4).rows[g9].offsetTop+this.GetViewport(e4).rows[g9].offsetHeight>this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
if (g9<this.GetViewport(e4).rows.length){
var j7=0;
while (g9>0){
j7+=this.GetViewport(e4).rows[g9].offsetHeight;
if (j7>this.GetViewport(e4).parentNode.offsetHeight){
break ;
}
g9--;
}
g9=this.GetPrevRow(e4,g9);
if (g9>=0){
this.GetViewport(e4).parentNode.scrollTop=this.GetViewport(e4).rows[g9].offsetTop;
e4.d3=g9;
e4.d2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
this.Select(e4,e4.d1,e4.d2);
}
}
}
break ;
}
this.SyncColSelection(e4);
}else {
if (key==this.tab){
if (event.shiftKey)key=this.left;
else key=this.right;
}
var x3=e4.d1;
var g9=e4.d3;
var h1=e4.d4;
switch (key){
case this.right:
if (event.keyCode==this.tab){
var x4=g9;
var x5=h1;
do {
this.MoveRight(e4,g9,h1);
g9=e4.d3;
h1=e4.d4;
}while (!(x4==g9&&x5==h1)&&this.GetCellFromRowCol(e4,g9,h1).getAttribute("TabStop")!=null&&this.GetCellFromRowCol(e4,g9,h1).getAttribute("TabStop")=="false")
}
else {
this.MoveRight(e4,g9,h1);
}
break ;
case this.left:
if (event.keyCode==this.tab){
var x4=g9;
var x5=h1;
do {
this.MoveLeft(e4,g9,h1);
g9=e4.d3;
h1=e4.d4;
}while (!(x4==g9&&x5==h1)&&this.GetCellFromRowCol(e4,g9,h1).getAttribute("TabStop")!=null&&this.GetCellFromRowCol(e4,g9,h1).getAttribute("TabStop")=="false")
}
else {
this.MoveLeft(e4,g9,h1);
}
break ;
case this.down:
this.MoveDown(e4,g9,h1);
break ;
case this.up:
this.MoveUp(e4,g9,h1);
break ;
case this.home:
if (e4.d1.parentNode.rowIndex>=0){
this.UpdateLeadingCell(e4,g9,0);
}
break ;
case this.end:
if (e4.d1.parentNode.rowIndex>=0){
h1=this.GetColCount(e4)-1;
this.UpdateLeadingCell(e4,g9,h1);
}
break ;
case this.pdn:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>=0){
g9=0;
for (g9=0;g9<this.GetViewport(e4).rows.length;g9++){
if (this.GetViewport(e4).rows[g9].offsetTop+this.GetViewport(e4).rows[g9].offsetHeight>this.GetViewport(e4).parentNode.offsetHeight+this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
g9=this.GetNextRow(e4,g9);
if (g9<this.GetViewport(e4).rows.length){
var f7=this.GetViewport(e4).rows[g9].offsetTop;
this.UpdateLeadingCell(e4,g9,e4.d4);
this.GetViewport(e4).parentNode.scrollTop=f7;
}else {
g9=this.GetPrevRow(e4,this.GetRowCount(e4)-1);
this.UpdateLeadingCell(e4,g9,e4.d4);
}
}
break ;
case this.pup:
if (this.GetViewport(e4)!=null&&e4.d1.parentNode.rowIndex>=0){
g9=0;
for (g9=0;g9<this.GetViewport(e4).rows.length;g9++){
if (this.GetViewport(e4).rows[g9].offsetTop+this.GetViewport(e4).rows[g9].offsetHeight>this.GetViewport(e4).parentNode.scrollTop){
break ;
}
}
if (g9<this.GetViewport(e4).rows.length){
var j7=0;
while (g9>=0){
j7+=this.GetViewport(e4).rows[g9].offsetHeight;
if (j7>this.GetViewport(e4).parentNode.offsetHeight){
break ;
}
g9--;
}
g9=this.GetPrevRow(e4,g9);
if (g9>=0){
var f7=this.GetViewport(e4).rows[g9].offsetTop;
this.UpdateLeadingCell(e4,g9,e4.d4);
this.GetViewport(e4).parentNode.scrollTop=f7;
}
}
}
break ;
}
if (x3!=e4.d1){
var g0=this.CreateEvent("ActiveCellChanged");
g0.cmdID=e4.id;
g0.Row=g0.row=this.GetSheetIndex(e4,this.GetRowFromCell(e4,e4.d1));
g0.Col=g0.col=this.GetColFromCell(e4,e4.d1);
if (e4.getAttribute("LayoutMode"))
g0.InnerRow=g0.innerRow=e4.d1.parentNode.getAttribute("row");
this.FireEvent(e4,g0);
}
}
var h2=this.GetCellFromRowCol(e4,e4.d3,e4.d4);
if (key==this.left&&h2.offsetLeft<x2.scrollLeft){
if (h2.cellIndex>0)
x2.scrollLeft=e4.d1.offsetLeft;
else 
x2.scrollLeft=0;
}else if (h2.cellIndex==0){
x2.scrollLeft=0;
}
if (key==this.right&&h2.offsetLeft+h2.offsetWidth>x2.scrollLeft+x2.offsetWidth-10){
x2.scrollLeft+=h2.offsetWidth;
}
if (key==this.up&&h2.parentNode.offsetTop<x2.scrollTop){
if (h2.parentNode.rowIndex>1)
x2.scrollTop=h2.parentNode.offsetTop;
else 
x2.scrollTop=0;
}else if (h2.parentNode.rowIndex==1){
x2.scrollTop=0;
}
var x6=this.GetParent(this.GetViewport(e4));
x2=this.GetParent(this.GetViewport(e4));
if (key==this.down&&this.IsChild(h2,x2)&&h2.offsetTop+h2.offsetHeight>x2.scrollTop+x2.clientHeight){
x6.scrollTop+=h2.offsetHeight;
}
if (h2!=null&&h2.offsetWidth<x2.clientWidth){
if (this.IsChild(h2,x2)&&h2.offsetLeft+h2.offsetWidth>x2.scrollLeft+x2.clientWidth){
x6.scrollLeft=h2.offsetLeft+h2.offsetWidth-x2.clientWidth;
}
}
if (this.IsChild(h2,x2)&&h2.offsetTop+h2.offsetHeight>x2.scrollTop+x2.clientHeight&&h2.offsetHeight<x2.clientHeight){
x6.scrollTop=h2.offsetTop+h2.offsetHeight-x2.clientHeight;
}
if (h2.offsetTop<x2.scrollTop){
x6.scrollTop=h2.offsetTop;
}
this.ScrollView(e4);
this.EnableButtons(e4);
this.SaveData(e4);
if (e4.d1!=null){
var i2=this.GetEditor(e4.d1);
if (i2!=null){
if (i2.tagName!="SELECT")
i2.focus();
if (!i2.disabled&&(i2.type==null||i2.type=="checkbox"||i2.type=="radio"||i2.type=="text"||i2.type=="password")){
this.a7=true;
this.a8=i2;
this.a9=this.GetEditorValue(i2);
}
}
}
this.Focus(e4);
}
this.MoveUp=function (e4,g9,h1){
var n0=this.GetRowCountInternal(e4);
var g8=this.GetColCount(e4);
g9--;
g9=this.GetPrevRow(e4,g9);
if (g9>=0){
e4.d3=g9;
this.UpdateLeadingCell(e4,e4.d3,e4.d4);
}
}
this.MoveDown=function (e4,g9,h1){
var n0=this.GetRowCountInternal(e4);
var g8=this.GetColCount(e4);
var q0=this.GetSpanCell(g9,h1,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (q0!=null){
g9=q0.row+q0.rowCount;
}else {
g9++;
}
g9=this.GetNextRow(e4,g9);
if (g9==n0)g9=n0-1;
if (g9<n0){
e4.d3=g9;
this.UpdateLeadingCell(e4,e4.d3,e4.d4);
}
}
this.MoveLeft=function (e4,g9,h1){
var x7=g9;
var n0=this.GetRowCountInternal(e4);
var g8=this.GetColCount(e4);
var q0=this.GetSpanCell(g9,h1,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (q0!=null){
h1=q0.col-1;
}else {
h1--;
}
if (h1<0){
h1=g8-1;
g9--;
if (g9<0){
g9=n0-1;
}
g9=this.GetPrevRow(e4,g9);
e4.d3=g9;
}
var x8=this.UpdateLeadingCell(e4,e4.d3,h1);
if (x8)e4.d3=x7;
}
this.MoveRight=function (e4,g9,h1){
var x7=g9;
var n0=this.GetRowCountInternal(e4);
var g8=this.GetColCount(e4);
var q0=this.GetSpanCell(g9,h1,this.GetSpanCells(e4,this.GetViewportFromCell(e4,e4.d1)));
if (q0!=null){
h1=q0.col+q0.colCount;
}else {
h1++;
}
if (h1>=g8){
h1=0;
g9++;
if (g9>=n0)g9=0;
e4.d3=this.GetNextRow(e4,g9);
}
var x8=this.UpdateLeadingCell(e4,e4.d3,h1);
if (x8)e4.d3=x7;
}
this.UpdateLeadingCell=function (e4,g9,h1){
var x9=0;
if (e4.getAttribute("LayoutMode")){
x9=this.GetRowFromViewPort(e4,g9,h1);
e4.d1=this.GetCellFromRowCol(e4,g9,h1);
var y0=this.GetCellFromRowCol(e4,x9,h1);
if (y0)x9=y0.parentNode.getAttribute("row");
}
var i1=this.FireActiveCellChangingEvent(e4,g9,h1,x9);
if (!i1){
var u9=this.GetOperationMode(e4);
if (u9!="MultiSelect")
this.ClearSelection(e4);
e4.d4=h1;
e4.d3=g9;
e4.d6=h1;
e4.d5=g9;
this.UpdateAnchorCell(e4,g9,h1);
}
return i1;
}
this.GetPrevRow=function (e4,g9){
if (g9<0)return 0;
var i3=this.GetViewport(e4);
if (i3!=null){
while (g9>0&&i3.rows[g9].cells.length>0){
if (this.IsChildSpreadRow(e4,i3,g9))
g9--;
else 
break ;
}
}
if (i3!=null&&g9>=0&&g9<i3.rows.length){
if (i3.rows[g9].getAttribute("previewrow")){
g9--;
}
}
return g9;
}
this.GetNextRow=function (e4,g9){
var i3=this.GetViewport(e4);
while (i3!=null&&g9<i3.rows.length){
if (this.IsChildSpreadRow(e4,i3,g9))g9++;
else 
break ;
}
if (i3!=null&&g9>=0&&g9<i3.rows.length){
if (i3.rows[g9].getAttribute("previewrow")){
g9++;
}
}
return g9;
}
this.FireActiveCellChangingEvent=function (e4,i5,m7,innerRow){
var g0=this.CreateEvent("ActiveCellChanging");
g0.cancel=false;
g0.cmdID=e4.id;
g0.row=this.GetSheetIndex(e4,i5);
g0.col=m7;
if (e4.getAttribute("LayoutMode"))
g0.innerRow=innerRow;
this.FireEvent(e4,g0);
return g0.cancel;
}
this.GetSheetRowIndex=function (e4,g9){
g9=this.GetDisplayIndex(e4,g9);
if (g9<0)return -1;
var l9=this.GetViewport(e4).rows[g9];
if (l9!=null){
return l9.getAttribute("FpKey");
}else {
return -1;
}
}
this.GetSheetColIndex=function (e4,h1){
var m7=-1;
var m6=null;
var y1=this.GetColHeader(e4);
if (y1!=null&&y1.rows.length>0){
m6=this.GetColGroup(y1);
}else {
var e5=this.GetViewport(e4);
if (e5!=null&&e5.rows.length>0){
m6=this.GetColGroup(e5);
}
}
if (m6!=null&&h1>=0&&h1<m6.childNodes.length){
m7=m6.childNodes[h1].getAttribute("FpCol");
}
return m7;
}
this.GetCellByRowCol=function (e4,g9,h1){
g9=this.GetDisplayIndex(e4,g9);
return this.GetCellFromRowCol(e4,g9,h1);
}
this.GetHeaderCellFromRowCol=function (e4,g9,h1,c6){
if (g9<0||h1<0)return null;
var e5=null;
if (c6){
e5=this.GetColHeader(e4);
}else {
e5=this.GetRowHeader(e4);
}
var q0=this.GetSpanCell(g9,h1,this.GetSpanCells(e4,e5));
if (q0!=null){
g9=q0.row;
h1=q0.col;
}
var y2=this.GetCellIndex(e4,g9,h1,this.GetSpanCells(e4,e5));
return e5.rows[g9].cells[y2];
}
this.GetCellFromRowCol=function (e4,g9,h1,prevCell){
if (g9<0||h1<0)return null;
var e5=null;
{
e5=this.GetViewport(e4);
}
var d9=e4.d9;
var q0=this.GetSpanCell(g9,h1,d9);
if (q0!=null){
g9=q0.row;
h1=q0.col;
}
var y2=0;
var w9=false;
if (e5!=null)w9=e5.parentNode.getAttribute("hiddenCells");
if (prevCell!=null&&!w9){
if (prevCell.cellIndex<prevCell.parentNode.cells.length-1)
y2=prevCell.cellIndex+1;
}
else 
{
y2=this.GetCellIndex(e4,g9,h1,d9);
}
if (g9>=0&&g9<e5.rows.length)
return e5.rows[g9].cells[y2];
else 
return null;
}
this.GetHiddenValue=function (e4,g9,colName){
if (colName==null)return ;
g9=this.GetDisplayIndex(e4,g9);
var s6=null;
var e5=null;
e5=this.GetViewport(e4);
if (e5!=null&&g9>=0&&g9<e5.rows.length){
var l9=e5.rows[g9];
s6=l9.getAttribute("hv"+colName);
}
return s6;
}
this.GetValue=function (e4,g9,h1){
g9=this.GetDisplayIndex(e4,g9);
var h2=this.GetCellFromRowCol(e4,g9,h1);
var j0=this.GetRender(h2);
var s6=this.GetValueFromRender(e4,j0);
if (s6!=null)s6=this.Trim(s6.toString());
return s6;
}
this.SetValue=function (e4,g9,h1,u2,noEvent,recalc){
g9=this.GetDisplayIndex(e4,g9);
if (u2!=null&&typeof(u2)!="string")u2=new String(u2);
var h2=this.GetCellFromRowCol(e4,g9,h1);
if (this.ValidateCell(e4,h2,u2)){
this.SetCellValueFromView(h2,u2);
if (u2!=null){
this.SetCellValue(e4,h2,""+u2,noEvent,recalc);
}else {
this.SetCellValue(e4,h2,"",noEvent,recalc);
}
this.SizeSpread(e4);
}else {
if (e4.getAttribute("lcidMsg")!=null)
alert(e4.getAttribute("lcidMsg"));
else 
alert("Can't set the data into the cell. The data type is not correct for the cell.");
}
}
this.SetActiveCell=function (e4,g9,h1){
this.ClearSelection(e4,true);
g9=this.GetDisplayIndex(e4,g9);
this.UpdateAnchorCell(e4,g9,h1);
this.ResetLeadingCell(e4);
}
this.GetOperationMode=function (e4){
var u9=e4.getAttribute("OperationMode");
return u9;
}
this.SetOperationMode=function (e4,u9){
e4.setAttribute("OperationMode",u9);
}
this.GetEnableRowEditTemplate=function (e4){
var y3=e4.getAttribute("EnableRowEditTemplate");
return y3;
}
this.GetSelectionPolicy=function (e4){
var y4=e4.getAttribute("SelectionPolicy");
return y4;
}
this.UpdateAnchorCell=function (e4,g9,h1,select){
if (g9<0||h1<0)return ;
e4.d1=this.GetCellFromRowCol(e4,g9,h1);
if (e4.d1==null)return ;
this.SetActiveRow(e4,this.GetRowKeyFromCell(e4,e4.d1));
this.SetActiveCol(e4,this.GetColKeyFromCell(e4,e4.d1));
if (select==null||select){
var u9=this.GetOperationMode(e4);
if (u9=="RowMode"||u9=="SingleSelect"||u9=="ExtendedSelect")
this.SelectRow(e4,g9,1,true,true);
else if (u9!="MultiSelect")
this.SelectRange(e4,g9,h1,1,1,true);
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
this.Edit=function (e4,i5){
var u9=this.GetOperationMode(e4);
if (u9!="RowMode")return ;
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5){
if (FarPoint&&FarPoint.System.WebControl.MultiColumnComboBoxCellTypeUtilitis)
FarPoint.System.WebControl.MultiColumnComboBoxCellTypeUtilitis.CloseAll();
this.SyncData(s3,"Edit,"+i5,e4);
}
else 
__doPostBack(s3,"Edit,"+i5);
}
this.Update=function (e4){
if (this.a7&&this.GetOperationMode(e4)!="RowMode"&&this.GetEnableRowEditTemplate(e4)!="true")return ;
this.SaveData(e4);
var s3=e4.getAttribute("name");
__doPostBack(s3,"Update");
}
this.Cancel=function (e4){
var f7=document.getElementById(e4.id+"_data");
f7.value="";
this.SaveData(e4);
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Cancel",e4);
else 
__doPostBack(s3,"Cancel");
}
this.Add=function (e4){
if (this.a7)return ;
var s3=null;
var o5=this.GetPageActiveSpread();
if (o5!=null){
s3=o5.getAttribute("name");
}else {
s3=e4.getAttribute("name");
}
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Add",e4);
else 
__doPostBack(s3,"Add");
}
this.Insert=function (e4){
if (this.a7)return ;
var s3=null;
var o5=this.GetPageActiveSpread();
if (o5!=null){
s3=o5.getAttribute("name");
}else {
s3=e4.getAttribute("name");
}
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Insert",e4);
else 
__doPostBack(s3,"Insert");
}
this.Delete=function (e4){
if (this.a7)return ;
var s3=null;
var o5=this.GetPageActiveSpread();
if (o5!=null){
s3=o5.getAttribute("name");
}else {
s3=e4.getAttribute("name");
}
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Delete",e4);
else 
__doPostBack(s3,"Delete");
}
this.Print=function (e4){
if (this.a7)return ;
this.SaveData(e4);
if (document.printSpread==null){
var f7=document.createElement("IFRAME");
f7.name="printSpread";
f7.style.position="absolute";
f7.style.left="-10px";
f7.style.width="0px";
f7.style.height="0px";
document.printSpread=f7;
document.body.insertBefore(f7,null);
f7.addEventListener("load",function (){the_fpSpread.PrintSpread();},false);
}
var y6=document.getElementsByTagName("FORM");
if (y6!=null&&y6.length>0){
var i1=y6[0];
i1.__EVENTTARGET.value=e4.getAttribute("name");
i1.__EVENTARGUMENT.value="Print";
var y7=i1.target;
i1.target="printSpread";
i1.submit();
i1.target=y7;
}
}
this.PrintSpread=function (){
document.printSpread.contentWindow.focus();
document.printSpread.contentWindow.print();
window.focus();
var o5=this.GetPageActiveSpread();
if (o5!=null)this.Focus(o5);
}
this.GotoPage=function (e4,e8){
if (this.a7)return ;
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Page,"+e8,e4);
else 
__doPostBack(s3,"Page,"+e8);
}
this.Next=function (e4){
if (this.a7)return ;
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Next",e4);
else 
__doPostBack(s3,"Next");
}
this.Prev=function (e4){
if (this.a7)return ;
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Prev",e4);
else 
__doPostBack(s3,"Prev");
}
this.GetViewportFromCell=function (e4,i8){
if (i8!=null){
var f7=i8;
while (f7!=null){
if (f7.tagName=="TABLE")break ;
f7=f7.parentNode;
}
if (f7==this.GetViewport(e4))
return f7;
}
return null;
}
this.IsChild=function (h2,i4){
if (h2==null||i4==null)return false;
var f7=h2.parentNode;
while (f7!=null){
if (f7==i4)return true;
f7=f7.parentNode;
}
return false;
}
this.GetCorner=function (e4){
return e4.c4;
}
this.Select=function (e4,cl1,cl2){
if (this.GetSpread(cl1)!=e4||this.GetSpread(cl2)!=e4)return ;
var h3=e4.d5;
var h4=e4.d6;
var y8=this.GetRowFromCell(e4,cl2);
var m0=0;
if (e4.d7=="r"){
m0=-1;
if (this.IsChild(cl2,this.GetColHeader(e4)))
y8=0;
}else if (e4.d7=="c"){
if (this.IsChild(cl2,this.GetRowHeader(e4)))
m0=0;
else 
m0=this.GetColFromCell(e4,cl2);
y8=-1;
}
else {
if (this.IsChild(cl2,this.GetColHeader(e4))){
y8=0;m0=this.GetColFromCell(e4,cl2);
}else if (this.IsChild(cl2,this.GetRowHeader(e4))){
m0=0;
}else {
m0=this.GetColFromCell(e4,cl2);
}
}
if (e4.d7=="t"){
h4=m0=h3=y8=-1;
}
var f7=Math.max(h3,y8);
h3=Math.min(h3,y8);
y8=f7;
f7=Math.max(h4,m0);
h4=Math.min(h4,m0);
m0=f7;
var h7=null;
var m8=this.GetSelection(e4);
var m9=m8.lastChild;
if (m9!=null){
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
h7=new this.Range();
this.SetRange(h7,"cell",g9,h1,n0,g8);
}
if (h7!=null&&h7.col==-1&&h7.row==-1)return ;
if (h7!=null&&h7.col==-1&&h7.row>=0){
if (h7.row>y8||h7.row+h7.rowCount-1<h3){
this.PaintSelection(e4,h7.row,h7.col,h7.rowCount,h7.colCount,false);
this.PaintSelection(e4,h3,h4,y8-h3+1,m0-h4+1,true);
}else {
if (h3>h7.row){
var f7=h3-h7.row;
this.PaintSelection(e4,h7.row,h7.col,f7,h7.colCount,false);
if (y8<h7.row+h7.rowCount-1){
this.PaintSelection(e4,y8,h7.col,h7.row+h7.rowCount-y8,h7.colCount,false);
}else {
this.PaintSelection(e4,h7.row+h7.rowCount,h7.col,y8-h7.row-h7.rowCount+1,h7.colCount,true);
}
}else {
this.PaintSelection(e4,h3,h7.col,h7.row-h3,h7.colCount,true);
if (y8<h7.row+h7.rowCount-1){
this.PaintSelection(e4,y8+1,h7.col,h7.row+h7.rowCount-y8-1,h7.colCount,false);
}else {
this.PaintSelection(e4,h7.row+h7.rowCount,h7.col,y8-h7.row-h7.rowCount+1,h7.colCount,true);
}
}
}
}else if (h7!=null&&h7.row==-1&&h7.col>=0){
if (h7.col>m0||h7.col+h7.colCount-1<h4){
this.PaintSelection(e4,h7.row,h7.col,h7.rowCount,h7.colCount,false);
this.PaintSelection(e4,h3,h4,y8-h3+1,m0-h4+1,true);
}else {
if (h4>h7.col){
this.PaintSelection(e4,h7.row,h7.col,h7.rowCount,h4-h7.col,false);
if (m0<h7.col+h7.colCount-1){
this.PaintSelection(e4,h7.row,m0,h7.rowCount,h7.col+h7.colCount-m0,false);
}else {
this.PaintSelection(e4,h7.row,h7.col+h7.colCount,h7.rowCount,m0-h7.col-h7.colCount,true);
}
}else {
this.PaintSelection(e4,h7.row,h4,h7.rowCount,h7.col-h4,true);
if (m0<h7.col+h7.colCount-1){
this.PaintSelection(e4,h7.row,m0+1,h7.rowCount,h7.col+h7.colCount-m0-1,false);
}else {
this.PaintSelection(e4,h7.row,h7.col+h7.colCount,h7.rowCount,m0-h7.col-h7.colCount+1,true);
}
}
}
}else if (h7!=null&&h7.row>=0&&h7.col>=0){
this.ExtendSelection(e4,h7,h3,h4,y8-h3+1,m0-h4+1);
}else {
this.PaintSelection(e4,h3,h4,y8-h3+1,m0-h4+1,true);
}
this.SetSelection(e4,h3,h4,y8-h3+1,m0-h4+1,h7==null);
}
this.ExtendSelection=function (e4,h7,newRow,newCol,newRowCount,newColCount)
{
var p8=Math.max(h7.col,newCol);
var p9=Math.min(h7.col+h7.colCount-1,newCol+newColCount-1);
var u0=Math.max(h7.row,newRow);
var y9=Math.min(h7.row+h7.rowCount-1,newRow+newRowCount-1);
if (h7.row<u0){
this.PaintSelection(e4,h7.row,h7.col,u0-h7.row,h7.colCount,false);
}
if (h7.col<p8){
this.PaintSelection(e4,h7.row,h7.col,h7.rowCount,p8-h7.col,false);
}
if (h7.row+h7.rowCount-1>y9){
this.PaintSelection(e4,y9+1,h7.col,h7.row+h7.rowCount-y9-1,h7.colCount,false);
}
if (h7.col+h7.colCount-1>p9){
this.PaintSelection(e4,h7.row,p9+1,h7.rowCount,h7.col+h7.colCount-p9-1,false);
}
if (newRow<u0){
this.PaintSelection(e4,newRow,newCol,u0-newRow,newColCount,true);
}
if (newCol<p8){
this.PaintSelection(e4,newRow,newCol,newRowCount,p8-newCol,true);
}
if (newRow+newRowCount-1>y9){
this.PaintSelection(e4,y9+1,newCol,newRow+newRowCount-y9-1,newColCount,true);
}
if (newCol+newColCount-1>p9){
this.PaintSelection(e4,newRow,p9+1,newRowCount,newCol+newColCount-p9-1,true);
}
}
this.PaintAnchorCellHeader=function (e4,select){
var g9,h1;
g9=this.GetRowFromCell(e4,e4.d1);
h1=this.GetColFromCell(e4,e4.d1);
if (select&&e4.d1.getAttribute("group")!=null){
var q0=this.GetSpanCell(g9,h1,e4.d9);
if (q0!=null&&q0.colCount>1){
var z0=this.GetSelectedRange(e4);
if (g9<z0.row||g9>=z0.row+z0.rowCount||h1<z0.col||h1>=z0.col+z0.colCount)
return ;
}
}
if (this.GetColHeader(e4)!=null)this.PaintHeaderSelection(e4,g9,h1,1,1,select,true);
if (this.GetRowHeader(e4)!=null)this.PaintHeaderSelection(e4,g9,h1,1,1,select,false);
}
this.LineIntersection=function (s1,h4,s2,m0){
var r3,g0;
r3=Math.max(s1,s2);
g0=Math.min(s1+h4,s2+m0);
if (r3<g0)
return {s:r3,c:g0-r3};
return null;
}
this.RangeIntersection=function (h3,h4,v3,cc1,y8,m0,rc2,cc2){
var z1=this.LineIntersection(h3,v3,y8,rc2);
var z2=this.LineIntersection(h4,cc1,m0,cc2);
if (z1&&z2)
return {row:z1.s,col:z2.s,rowCount:z1.c,colCount:z2.c};
return null;
}
this.PaintSelection=function (e4,g9,h1,n0,g8,select){
if (g9<0&&h1<0){
this.PaintCornerSelection(e4,select);
}
var z3=false;
var z4=false;
if (g9<0){
g9=0;
n0=this.GetRowCountInternal(e4);
}
if (h1<0){
h1=0;
g8=this.GetColCount(e4);
}
this.PaintViewportSelection(e4,g9,h1,n0,g8,select);
var m8=this.GetSelection(e4);
var m9;
var y8;
var m0;
var z5;
var z6;
var h7;
var z7;
for (var e9=m8.childNodes.length-1;e9>=0;e9--){
m9=m8.childNodes[e9];
if (m9){
y8=parseInt(m9.getAttribute("rowIndex"));
m0=parseInt(m9.getAttribute("colIndex"));
z5=parseInt(m9.getAttribute("rowcount"));
z6=parseInt(m9.getAttribute("colcount"));
if (y8<0||z5<0){y8=0;z5=this.GetRowCountInternal(e4);}
if (m0<0||z6<0){m0=0;z6=this.GetColCount(e4);}
if (e9>=m8.childNodes.length-1){
if (g9<=y8&&n0>=z5){
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal"){
this.PaintHeaderSelection(e4,g9,h1,n0,g8,select,true);
z3=true;
}
}
if (h1<=m0&&g8>=z6){
if (this.GetRowHeader(e4)!=null){
this.PaintHeaderSelection(e4,g9,h1,n0,g8,select,false);
z4=true;
}
}
if (!z3&&!z4){
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal"){
this.PaintHeaderSelection(e4,g9,h1,n0,g8,select,true);
z3=true;
}
if (this.GetRowHeader(e4)!=null){
this.PaintHeaderSelection(e4,g9,h1,n0,g8,select,false);
z4=true;
}
}
}
else {
if (!select&&this.GetOperationMode(e4)=="Normal"){
h7=this.RangeIntersection(g9,h1,n0,g8,y8,m0,z5,z6);
if (h7){
this.PaintViewportSelection(e4,h7.row,h7.col,h7.rowCount,h7.colCount,true);
}
if (z3){
z7=this.LineIntersection(h1,g8,m0,z6);
if (z7)this.PaintHeaderSelection(e4,g9,z7.s,n0,z7.c,true,true);
}
if (z4){
z7=this.LineIntersection(g9,n0,y8,z5);
if (z7)this.PaintHeaderSelection(e4,z7.s,h1,z7.c,g8,true,false);
}
}
}
}
}
if (m8.childNodes.length<=0){
if (this.GetColHeader(e4)!=null&&this.GetOperationMode(e4)=="Normal")this.PaintHeaderSelection(e4,g9,h1,n0,g8,select,true);
if (this.GetRowHeader(e4)!=null)this.PaintHeaderSelection(e4,g9,h1,n0,g8,select,false);
}
this.PaintAnchorCell(e4);
}
this.PaintFocusRect=function (e4){
var g3=document.getElementById(e4.id+"_focusRectT");
if (g3==null)return ;
var z8=this.GetSelectedRange(e4);
if (e4.d1==null&&(z8==null||(z8.rowCount==0&&z8.colCount==0))){
g3.style.left="-1000px";
var s3=e4.id;
g3=document.getElementById(s3+"_focusRectB");
g3.style.left="-1000px";
g3=document.getElementById(s3+"_focusRectL");
g3.style.left="-1000px";
g3=document.getElementById(s3+"_focusRectR");
g3.style.left="-1000px";
return ;
}
var i0=this.GetOperationMode(e4);
if (i0=="RowMode"||i0=="SingleSelect"||i0=="MultiSelect"||i0=="ExtendedSelect"){
var g9=e4.GetActiveRow();
z8=new this.Range();
this.SetRange(z8,"Row",g9,-1,1,-1);
}else if (z8==null||(z8.rowCount==0&&z8.colCount==0)){
var g9=e4.GetActiveRow();
var h1=e4.GetActiveCol();
z8=new this.Range();
this.SetRange(z8,"Cell",g9,h1,e4.d1.rowSpan,e4.d1.colSpan);
}
if (z8.row<0){
z8.row=0;
z8.rowCount=this.GetRowCountInternal(e4);
}
if (z8.col<0){
z8.col=0;
z8.colCount=this.GetColCount(e4);
}
var h2=this.GetCellFromRowCol(e4,z8.row,z8.col);
if (h2==null)return ;
if (z8.rowCount==1&&z8.colCount==1){
z8.rowCount=h2.rowSpan;
z8.colCount=h2.colSpan;
if (h2.colSpan>1){
var z9=parseInt(h2.getAttribute("col"));
if (z9!=z8.col&&!isNaN(z9))z8.col=z9;
}
}
var f7=this.GetOffsetTop(e4,h2);
var aa0=this.GetOffsetLeft(e4,h2);
if (h2.rowSpan>1){
z8.row=h2.parentNode.rowIndex;
var h4=this.GetCellFromRowCol(e4,z8.row,z8.col+z8.colCount-1);
if (h4!=null&&h4.parentNode.rowIndex>h2.parentNode.rowIndex){
f7=this.GetOffsetTop(e4,h4);
}
}
if (h2.colSpan>1){
var h4=this.GetCellFromRowCol(e4,z8.row+z8.rowCount-1,z8.col);
var j6=this.GetOffsetLeft(e4,h4);
if (j6>aa0){
aa0=j6;
h2=h4;
}
}
var j7=0;
var g7=this.GetViewport(e4).rows;
for (var g9=z8.row;g9<z8.row+z8.rowCount&&g9<g7.length;g9++){
j7+=g7[g9].offsetHeight;
if (g9>z8.row)j7+=parseInt(this.GetViewport(e4).cellSpacing);
}
var i9=0;
var m6=this.GetColGroup(this.GetViewport(e4));
if (m6.childNodes==null||m6.childNodes.length==0)return ;
for (var h1=z8.col;h1<z8.col+z8.colCount&&h1<m6.childNodes.length;h1++){
i9+=m6.childNodes[h1].offsetWidth;
if (h1>z8.col)i9+=parseInt(this.GetViewport(e4).cellSpacing);
}
if (z8.col>h2.cellIndex&&z8.type=="Column"){
var m0=parseInt(h2.getAttribute("col"));
for (var h1=m0;h1<z8.col;h1++){
aa0+=m6.childNodes[h1].offsetWidth;
if (h1>m0)aa0+=parseInt(this.GetViewport(e4).cellSpacing);
}
}
if (z8.row>0)f7-=2;
else j7-=2;
if (z8.col>0)aa0-=2;
else i9-=2;
if (parseInt(this.GetViewport(e4).cellSpacing)>0){
f7+=1;aa0+=1;
}else {
i9+=1;
j7+=1;
}
if (i9<0)i9=0;
if (j7<0)j7=0;
g3.style.left=""+aa0+"px";
g3.style.top=""+f7+"px";
g3.style.width=""+i9+"px";
g3=document.getElementById(e4.id+"_focusRectB");
g3.style.left=""+aa0+"px";
g3.style.top=""+(f7+j7)+"px";
g3.style.width=""+i9+"px";
g3=document.getElementById(e4.id+"_focusRectL");
g3.style.left=""+aa0+"px";
g3.style.top=""+f7+"px";
g3.style.height=""+j7+"px";
g3=document.getElementById(e4.id+"_focusRectR");
g3.style.left=""+(aa0+i9)+"px";
g3.style.top=""+f7+"px";
g3.style.height=""+j7+"px";
}
this.PaintCornerSelection=function (e4,select){
var aa1=true;
if (e4.getAttribute("ShowHeaderSelection")=="false")aa1=false;
if (!aa1)return ;
var m5=this.GetCorner(e4);
if (m5!=null&&m5.rows.length>0){
for (var e9=0;e9<m5.rows.length;e9++){
for (var h9=0;h9<m5.rows[0].cells.length;h9++){
if (m5.rows[e9].cells[h9]!=null)
this.PaintSelectedCell(e4,m5.rows[e9].cells[h9],select);
}
}
}
}
this.PaintHeaderSelection=function (e4,g9,h1,n0,g8,select,c6){
var aa1=true;
if (e4.getAttribute("ShowHeaderSelection")=="false")aa1=false;
if (!aa1)return ;
var aa2=this.GetRowCountInternal(e4);
var aa3=this.GetColCount(e4);
if (c6){
if (this.GetColHeader(e4)==null)return ;
g9=0;
n0=aa2=this.GetColHeader(e4).rows.length;
}else {
if (this.GetRowHeader(e4)==null)return ;
h1=0;
g8=aa3=this.GetColGroup(this.GetRowHeader(e4)).childNodes.length;
}
var aa4=c6?e4.e1:e4.e0;
for (var e9=g9;e9<g9+n0&&e9<aa2;e9++){
if (!c6&&this.IsChildSpreadRow(e4,this.GetViewport(e4),e9))continue ;
for (var h9=h1;h9<h1+g8&&h9<aa3;h9++){
if (this.IsCovered(e4,e9,h9,aa4))continue ;
var h2=this.GetHeaderCellFromRowCol(e4,e9,h9,c6);
if (h2!=null)this.PaintSelectedCell(e4,h2,select);
}
}
}
this.PaintViewportSelection=function (e4,g9,h1,n0,g8,select){
var aa2=this.GetRowCountInternal(e4);
var aa3=this.GetColCount(e4);
for (var e9=g9;e9<g9+n0&&e9<aa2;e9++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),e9))continue ;
var h2=null;
for (var h9=h1;h9<h1+g8&&h9<aa3;h9++){
if (this.IsCovered(e4,e9,h9,e4.d9))continue ;
h2=this.GetCellFromRowCol(e4,e9,h9,h2);
this.PaintSelectedCell(e4,h2,select);
}
}
}
this.Copy=function (e4){
var o5=this.GetPageActiveSpread();
if (o5!=null&&o5!=e4&&this.GetTopSpread(o5)==e4){
this.Copy(o5);
return ;
}
var m8=this.GetSelection(e4);
var m9=m8.lastChild;
if (m9!=null){
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
if (g9<0){
g9=0;
n0=this.GetRowCountInternal(e4);
}
if (h1<0){
h1=0;
g8=this.GetColCount(e4);
}
var f3="";
for (var e9=g9;e9<g9+n0;e9++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),e9))continue ;
var h2=null;
for (var h9=h1;h9<h1+g8;h9++){
if (this.IsCovered(e4,e9,h9,e4.d9))
f3+="";
else 
{
h2=this.GetCellFromRowCol(e4,e9,h9,h2);
if (h2!=null&&h2.parentNode.getAttribute("previewrow")=="true")continue ;
var j1=this.GetCellType(h2);
if (j1=="TextCellType"&&h2.getAttribute("password")!=null)
f3+="";
else 
f3+=this.GetCellValueFromView(e4,h2);
}
if (h9+1<h1+g8)f3+="\t";
}
f3+="\r\n";
}
this.b9=f3;
}else {
if (e4.d1!=null){
var f3=this.GetCellValueFromView(e4,e4.d1);
this.b9=f3;
}
}
}
this.GetCellValueFromView=function (e4,h2){
var u2=null;
if (h2!=null){
var aa5=this.GetRender(h2);
u2=this.GetValueFromRender(e4,aa5);
if (u2==null||u2==" ")u2="";
}
return u2;
}
this.SetCellValueFromView=function (h2,u2,ignoreLock){
if (h2!=null){
var aa5=this.GetRender(h2);
var r2=this.GetCellType(h2);
if ((r2!="readonly"||ignoreLock)&&aa5!=null&&aa5.getAttribute("FpEditor")!="Button")
this.SetValueToRender(aa5,u2);
}
}
this.Paste=function (e4){
var o5=this.GetPageActiveSpread();
if (o5!=null&&o5!=e4&&this.GetTopSpread(o5)==e4){
this.Paste(o5);
return ;
}
if (e4.d1==null)return ;
var f3=this.b9;
if (f3==null)return ;
var e5=this.GetViewportFromCell(e4,e4.d1);
var g9=this.GetRowFromCell(e4,e4.d1);
var h1=this.GetColFromCell(e4,e4.d1);
var g8=this.GetColCount(e4);
var n0=this.GetRowCountInternal(e4);
var aa6=g9;
var w8=h1;
var aa7=new String(f3);
if (aa7.length==0)return ;
var e8=aa7.lastIndexOf("\r\n");
if (e8>=0&&e8==aa7.length-2)aa7=aa7.substring(0,e8);
var aa8=0;
var aa9=aa7.split("\r\n");
for (var e9=0;e9<aa9.length&&aa6<n0;e9++){
if (typeof(aa9[e9])=="string"){
aa9[e9]=aa9[e9].split("\t");
if (aa9[e9].length>aa8)aa8=aa9[e9].length;
}
aa6++;
}
aa6=this.GetSheetIndex(e4,g9);
for (var e9=0;e9<aa9.length&&aa6<n0;e9++){
var ab0=aa9[e9];
if (ab0!=null){
w8=h1;
var h2=null;
var y8=this.GetDisplayIndex(e4,aa6);
for (var h9=0;h9<ab0.length&&w8<g8;h9++){
if (!this.IsCovered(e4,y8,w8,e4.d9)){
h2=this.GetCellFromRowCol(e4,y8,w8,h2);
if (h2!=null&&h2.parentNode.getAttribute("previewrow")=="true")continue ;
if (h2==null)return ;
var ab1=ab0[h9];
if (!this.ValidateCell(e4,h2,ab1)){
if (e4.getAttribute("lcidMsg")!=null)
alert(e4.getAttribute("lcidMsg"));
else 
alert("Can't set the data into the cell. The data type is not correct for the cell.");
return ;
}
}
w8++;
}
}
aa6++;
}
if (aa9.length==0)return ;
aa6=this.GetSheetIndex(e4,g9);
for (var e9=0;e9<aa9.length&&aa6<n0;e9++){
w8=h1;
var ab0=aa9[e9];
var h2=null;
var y8=this.GetDisplayIndex(e4,aa6);
for (var h9=0;h9<aa8&&w8<g8;h9++){
if (!this.IsCovered(e4,y8,w8,e4.d9)){
h2=this.GetCellFromRowCol(e4,y8,w8,h2);
if (h2!=null&&h2.parentNode.getAttribute("previewrow")=="true")continue ;
var r2=this.GetCellType(h2);
var aa5=this.GetRender(h2);
if (r2!="readonly"&&aa5.getAttribute("FpEditor")!="Button"){
var ab1=null;
if (ab0!=null&&h9<ab0.length)ab1=ab0[h9];
this.SetCellValueFromView(h2,ab1);
if (ab1!=null){
this.SetCellValue(e4,h2,""+ab1);
}else {
this.SetCellValue(e4,h2,"");
}
}
}
w8++;
}
aa6++;
}
var u7=e4.getAttribute("autoCalc");
if (u7!="false"){
this.UpdateValues(e4);
}
var e7=this.GetTopSpread(e4);
var f8=document.getElementById(e7.id+"_textBox");
if (f8!=null){
f8.blur();
}
this.Focus(e4);
}
this.UpdateValues=function (e4){
if (e4.d8==null&&this.GetParentSpread(e4)==null&&e4.getAttribute("rowFilter")!="true"&&e4.getAttribute("hierView")!="true"&&e4.getAttribute("IsNewRow")!="true"){
this.SaveData(e4);
this.StorePostData(e4);
this.SyncData(e4.getAttribute("name"),"UpdateValues",e4);
}
}
this.ValidateCell=function (e4,h2,u2){
if (h2==null||u2==null||u2=="")return true;
var u5=null;
var j1=this.GetCellType(h2);
if (j1!=null){
var i1=this.GetFunction(j1+"_isValid");
if (i1!=null){
u5=i1(h2,u2);
}
}
return (u5==null||u5=="");
}
this.DoclearSelection=function (e4){
var m8=this.GetSelection(e4);
var m9=m8.lastChild;
while (m9!=null){
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
this.PaintSelection(e4,g9,h1,n0,g8,false);
m8.removeChild(m9);
m9=m8.lastChild;
}
}
this.Clear=function (e4){
var o5=this.GetPageActiveSpread();
if (o5!=null&&o5!=e4&&this.GetTopSpread(o5)==e4){
this.Clear(o5);
return ;
}
var r2=this.GetCellType(e4.d1);
if (r2=="readonly")return ;
var m8=this.GetSelection(e4);
var m9=m8.lastChild;
if (this.AnyReadOnlyCell(e4,m9)){
return ;
}
this.Copy(e4);
if (m9!=null){
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
if (g9<0){
g9=0;
n0=this.GetRowCountInternal(e4);
}
if (h1<0){
h1=0;
g8=this.GetColCount(e4);
}
for (var e9=g9;e9<g9+n0;e9++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),e9))continue ;
var h2=null;
for (var h9=h1;h9<h1+g8;h9++){
if (!this.IsCovered(e4,e9,h9,e4.d9)){
h2=this.GetCellFromRowCol(e4,e9,h9,h2);
if (h2!=null&&h2.parentNode.getAttribute("previewrow")=="true")continue ;
var r2=this.GetCellType(h2);
if (r2!="readonly"){
var ab2=this.GetEditor(h2);
if (ab2!=null&&ab2.getAttribute("FpEditor")=="Button")continue ;
this.SetCellValueFromView(h2,null);
this.SetCellValue(e4,h2,"");
}
}
}
}
var u7=e4.getAttribute("autoCalc");
if (u7!="false"){
this.UpdateValues(e4);
}
}
}
this.AnyReadOnlyCell=function (e4,m9){
if (m9!=null){
var g9=this.GetRowByKey(e4,m9.getAttribute("row"));
var h1=this.GetColByKey(e4,m9.getAttribute("col"));
var n0=parseInt(m9.getAttribute("rowcount"));
var g8=parseInt(m9.getAttribute("colcount"));
if (g9<0){
g9=0;
n0=this.GetRowCountInternal(e4);
}
if (h1<0){
h1=0;
g8=this.GetColCount(e4);
}
for (var e9=g9;e9<g9+n0;e9++){
if (this.IsChildSpreadRow(e4,this.GetViewport(e4),e9))continue ;
var h2=null;
for (var h9=h1;h9<h1+g8;h9++){
if (!this.IsCovered(e4,e9,h9,e4.d9)){
h2=this.GetCellFromRowCol(e4,e9,h9,h2);
var r2=this.GetCellType(h2);
if (r2=="readonly"){
return true;
}
}
}
}
}
return false;
}
this.MoveSliderBar=function (e4,g0){
var l8=this.GetElementById(this.activePager,e4.id+"_slideBar");
var f7=(g0.clientX-this.GetOffsetLeft(e4,e4,document.body)+window.scrollX-8);
if (f7<e4.slideLeft)f7=e4.slideLeft;
if (f7>e4.slideRight)f7=e4.slideRight;
var m2=parseInt(this.activePager.getAttribute("totalPage"))-1;
var ab3=parseInt(((f7-e4.slideLeft)/(e4.slideRight-e4.slideLeft))*m2)+1;
if (e4.style.position!="absolute"&&e4.style.position!="relative")
f7+=this.GetOffsetLeft(e4,e4,document.body)
l8.style.left=f7+"px";
return ab3;
}
this.MouseMove=function (event){
if (window.fpPostOn!=null)return ;
event=this.GetEvent(event);
var n7=this.GetTarget(event);
if (n7!=null&&n7.tagName=="scrollbar")
return ;
var e4=this.GetSpread(n7,true);
if (e4!=null&&this.dragSlideBar)
{
if (this.activePager!=null){
var ab3=this.MoveSliderBar(e4,event);
var ab4=this.GetElementById(this.activePager,e4.id+"_posIndicator");
ab4.innerHTML=this.activePager.getAttribute("pageText")+ab3;
}
return ;
}
if (this.a6)e4=this.GetSpread(this.b8);
if (e4==null||(!this.a6&&this.HitCommandBar(n7)))return ;
if (e4.getAttribute("OperationMode")=="ReadOnly")return ;
var j8=this.IsXHTML(e4);
if (this.a6){
if (this.dragCol!=null&&this.dragCol>=0){
var v6=this.GetMovingCol(e4);
if (v6!=null){
if (v6.style.display=="none")v6.style.display="";
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
v6.style.top=""+(event.clientY+window.scrollY)+"px";
v6.style.left=""+(event.clientX+window.scrollX+5)+"px";
}else {
v6.style.top=""+(event.clientY-this.GetOffsetTop(e4,e4,document.body)+window.scrollY)+"px";
v6.style.left=""+(event.clientX-this.GetOffsetLeft(e4,e4,document.body)+window.scrollX+5)+"px";
}
}
var e5=this.GetViewport(e4);
var ab5=document.body;
var ab6=this.GetGroupBar(e4);
var f7=-1;
var l7=event.clientX;
var u0=0;
var p8=0;
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
u0=this.GetOffsetTop(e4,e4,document.body)-e5.parentNode.scrollTop;
p8=this.GetOffsetLeft(e4,e4,document.body)-e5.parentNode.scrollLeft;
l7+=Math.max(document.body.scrollLeft,document.documentElement.scrollLeft);
}else {
l7-=(this.GetOffsetLeft(e4,e4,document.body)-Math.max(document.body.scrollLeft,document.documentElement.scrollLeft));
}
var ab7=false;
var j8=this.IsXHTML(e4);
var ab8=j8?document.body.parentNode.scrollTop:document.body.scrollTop;
var k8=document.getElementById(e4.id+"_titleBar");
if (k8)ab8-=k8.parentNode.parentNode.offsetHeight;
if (this.GetPager1(e4)!=null)ab8-=this.GetPager1(e4).offsetHeight;
if (ab6!=null&&event.clientY<this.GetOffsetTop(e4,e4,document.body)-e5.parentNode.scrollTop+ab6.offsetHeight-ab8){
if (e4.style.position!="absolute"&&e4.style.position!="relative")
p8=this.GetOffsetLeft(e4,e4,document.body);
u0+=10;
ab7=true;
var v9=ab6.getElementsByTagName("TABLE")[0];
if (v9!=null){
for (var e9=0;e9<v9.rows[0].cells[0].childNodes.length;e9++){
var i9=v9.rows[0].cells[0].childNodes[e9].offsetWidth;
if (i9==null)continue ;
if (p8<=l7&&l7<p8+i9){
f7=e9;
break ;
}
p8+=i9;
}
}
if (f7==-1&&l7>=p8)f7=-2;
e4.targetCol=f7;
}else {
if (e4.style.position=="absolute"||e4.style.position=="relative")
p8=-e5.parentNode.scrollLeft;
if (this.GetRowHeader(e4)!=null)p8+=this.GetRowHeader(e4).offsetWidth;
if (ab6!=null)u0+=ab6.offsetHeight;
if (l7<p8){
f7=0;
}else {
var m6=this.GetColGroup(this.GetColHeader(e4));
if (m6!=null){
for (var e9=0;e9<m6.childNodes.length;e9++){
var i9=parseInt(m6.childNodes[e9].width);
if (i9==null)continue ;
if (p8<=l7&&l7<p8+i9){
f7=e9;
break ;
}
p8+=i9;
}
}
}
if (f7>=0&&f7!=this.dragViewCol){
if (this.dragViewCol<f7){
f7++;
if (f7<m6.childNodes.length)
p8+=i9;
}
}
p8-=5;
var ab9=parseInt(this.GetSheetColIndex(e4,f7));
if (ab9<0)ab9=f7;
e4.targetCol=ab9;
}
if (k8)u0+=k8.parentNode.parentNode.offsetHeight;
if (this.GetPager1(e4)!=null)u0+=this.GetPager1(e4).offsetHeight;
var ab4=this.GetPosIndicator(e4);
ab4.style.left=""+p8+"px";
ab4.style.top=""+u0+"px";
if (ab6!=null&&ab7&&ab6.getElementsByTagName("TABLE").length==0){
ab4.style.display="none";
}else {
if (ab7||e4.allowColMove)
ab4.style.display="";
else 
ab4.style.display="none";
}
return ;
}
if (this.b4==null&&this.b5==null){
if (e4.d1!=null){
var i3=this.GetParent(this.GetViewport(e4));
if (i3!=null){
var r4=e4.offsetTop+i3.offsetTop+i3.offsetHeight-10;
if (event.clientY>r4){
i3.scrollTop=i3.scrollTop+10;
this.ScrollView(e4);
}else if (event.clientY<e4.offsetTop+i3.offsetTop+5){
i3.scrollTop=i3.scrollTop-10;
this.ScrollView(e4);
}
var ac0=e4.offsetLeft+i3.offsetLeft+i3.offsetWidth-20;
if (event.clientX>ac0){
i3.scrollLeft=i3.scrollLeft+10;
this.ScrollView(e4);
}else if (event.clientX<e4.offsetLeft+i3.offsetLeft+5){
i3.scrollLeft=i3.scrollLeft-10;
this.ScrollView(e4);
}
}
var h2=this.GetCell(n7,null,event);
if (h2!=null&&h2!=e4.d2){
var i0=this.GetOperationMode(e4);
if (i0!="MultiSelect"){
if (i0=="SingleSelect"||i0=="RowMode"){
this.ClearSelection(e4);
var h3=this.GetRowFromCell(e4,h2);
this.UpdateAnchorCell(e4,h3,0);
this.SelectRow(e4,h3,1,true,true);
}else {
if (!(i0=="Normal"&&this.GetSelectionPolicy(e4)=="Single")){
this.Select(e4,e4.d1,h2);
this.SyncColSelection(e4);
}
}
e4.d2=h2;
}
}
}
}else if (this.b4!=null){
var ac1=event.clientX-this.b6;
var w6=parseInt(this.b4.width)+ac1;
var t9=0;
var ac2=(w6>t9);
if (ac2){
this.b4.width=w6;
var k3=parseInt(this.b4.getAttribute("index"));
this.SetWidthFix(this.GetColHeader(e4),k3,w6);
this.b6=event.clientX;
}
}else if (this.b5!=null){
var ac1=event.clientY-this.b7;
var ac3=parseInt(this.b5.style.height)+ac1;
var t9=0;
var ac2=(t9<ac3);
if (ac2){
this.b5.cells[0].style.posHeight=this.b5.cells[1].style.posHeight=(this.b5.cells[0].style.posHeight+ac1);
this.b7=event.clientY;
}
}
}else {
this.b8=n7;
if (this.b8==null||this.GetSpread(this.b8)!=e4)return ;
var n7=this.GetSizeColumn(e4,this.b8,event);
if (n7!=null){
this.b4=n7;
this.b8.style.cursor=this.GetResizeCursor(false);
}else {
var n7=this.GetSizeRow(e4,this.b8,event);
if (n7!=null){
this.b5=n7;
if (this.b8!=null&&this.b8.style!=null)this.b8.style.cursor=this.GetResizeCursor(true);
}else {
if (this.b8!=null&&this.b8.style!=null){
var h2=this.GetCell(this.b8);
if (h2!=null&&this.IsHeaderCell(e4,h2))this.b8.style.cursor="default";
if (this.b8!=null&&(this.b8.getAttribute("FpSpread")=="rowpadding"||this.b8.getAttribute("ControlType")=="chgrayarea"))
this.b8.style.cursor=this.GetgrayAreaCursor(e4);
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
this.GetResizeCursor=function (i5){
if (i5){
return "n-resize";
}else {
return "w-resize";
}
}
this.HitCommandBar=function (n7){
var f7=n7;
var e4=this.GetTopSpread(this.GetSpread(f7,true));
if (e4==null)return false;
var p4=this.GetCommandBar(e4);
while (f7!=null&&f7!=e4){
if (f7==p4)return true;
f7=f7.parentNode;
}
return false;
}
this.OpenWaitMsg=function (e4){
var i1=document.getElementById(e4.id+"_waitmsg");
if (i1==null)return ;
var i9=e4.offsetWidth;
var j7=e4.offsetHeight;
var i6=this.CreateTestBox(e4);
i6.style.fontFamily=i1.style.fontFamily;
i6.style.fontSize=i1.style.fontSize;
i6.style.fontWeight=i1.style.fontWeight;
i6.style.fontStyle=i1.style.fontStyle;
i6.innerHTML=i1.innerHTML;
i1.style.width=""+(i6.offsetWidth+2)+"px";
var aa0=Math.max(10,(i9-parseInt(i1.style.width))/2);
var f7=Math.max(10,(j7-parseInt(i1.style.height))/2);
if (e4.style.position!="absolute"&&e4.style.position!="relative"){
aa0+=e4.offsetLeft;
f7+=e4.offsetTop;
}
i1.style.top=""+f7+"px";
i1.style.left=""+aa0+"px";
i1.style.display="block";
}
this.CloseWaitMsg=function (e4){
var i1=document.getElementById(e4.id+"_waitmsg");
if (i1==null)return ;
i1.style.display="none";
this.Focus(e4);
}
this.MouseDown=function (event){
if (window.fpPostOn!=null)return ;
event=this.GetEvent(event);
var n7=this.GetTarget(event);
var e4=this.GetSpread(n7,true);
e4.mouseY=event.clientY;
var ac4=this.GetPageActiveSpread();
if (this.GetViewport(e4)==null)return ;
if (e4!=null&&n7.parentNode!=null&&n7.parentNode.getAttribute("name")==e4.id+"_slideBar"){
if (this.IsChild(n7,this.GetPager1(e4)))
this.activePager=this.GetPager1(e4);
else if (this.IsChild(n7,this.GetPager2(e4)))
this.activePager=this.GetPager2(e4);
if (this.activePager!=null){
var o0=true;
if (this.a7)o0=this.EndEdit(e4);
if (o0){
this.UpdatePostbackData(e4);
this.dragSlideBar=true;
}
}
return this.CancelDefault(event);
}
if (this.GetOperationMode(e4)=="ReadOnly")return ;
var j8=false;
if (e4!=null)j8=this.IsXHTML(e4);
if (this.a7&&e4.getAttribute("mcctCellType")!="true"){
var f7=this.GetCell(n7);
if (f7!=e4.d1){
var o0=this.EndEdit();
if (!o0)return ;
}else 
return ;
}
if (n7==this.GetParent(this.GetViewport(e4))){
if (this.GetTopSpread(ac4)!=e4){
this.SetActiveSpread(event);
}
return ;
}
var ac5=(ac4==e4);
this.SetActiveSpread(event);
ac4=this.GetPageActiveSpread();
if (this.HitCommandBar(n7))return ;
if (event.button==2)return ;
if (this.IsChild(n7,this.GetGroupBar(e4))){
var h4=parseInt(n7.id.replace(e4.id+"_group",""));
if (!isNaN(h4)){
this.dragCol=h4;
this.dragViewCol=this.GetColByKey(e4,h4);
var v6=this.GetMovingCol(e4);
v6.innerHTML=n7.innerHTML;
v6.style.width=""+Math.max(this.GetPreferredCellWidth(e4,n7),80)+"px";
if (e4.getAttribute("DragColumnCssClass")==null)
v6.style.backgroundColor=n7.style.backgroundColor;
v6.style.top="-50px";
v6.style.left="-100px";
this.a6=true;
e4.dragFromGroupbar=true;
this.CancelDefault(event);
return ;
}
}
this.b4=this.GetSizeColumn(e4,n7,event);
if (this.b4!=null){
this.a6=true;
this.b6=this.b7=event.clientX;
if (this.b4.style!=null)this.b4.style.cursor=this.GetResizeCursor(false);
this.b8=n7;
}else {
this.b5=this.GetSizeRow(e4,n7,event);
if (this.b5!=null){
this.a6=true;
this.b6=this.b7=event.clientY;
this.b5.style.cursor=this.GetResizeCursor(true);
this.b8=n7;
}else {
var ac6=this.GetCell(n7,null,event);
if (ac6==null){
var c4=this.GetCorner(e4);
if (c4!=null&&this.IsChild(n7,c4)){
if (this.GetOperationMode(e4)=="Normal")
this.SelectTable(e4,true);
}
return ;
}
var ac7=this.GetColFromCell(e4,ac6);
if (ac6.parentNode.getAttribute("FpSpread")=="ch"&&this.GetColFromCell(e4,ac6)>=this.GetColCount(e4))return ;
if (ac6.parentNode.getAttribute("FpSpread")=="rh"&&this.IsChildSpreadRow(e4,this.GetViewport(e4),ac6.parentNode.rowIndex))return ;
if (ac6.parentNode.getAttribute("FpSpread")=="ch"&&(this.GetOperationMode(e4)=="RowMode"||this.GetOperationMode(e4)=="SingleSelect"||this.GetOperationMode(e4)=="ExtendedSelect")){
if (!e4.allowColMove&&!e4.allowGroup)
return ;
}else {
var n8=this.FireActiveCellChangingEvent(e4,this.GetRowFromCell(e4,ac6),this.GetColFromCell(e4,ac6),ac6.parentNode.getAttribute("row"));
if (n8)return ;
var u9=this.GetOperationMode(e4);
var e7=this.GetTopSpread(e4);
if (!event.ctrlKey||e4.getAttribute("multiRange")!="true"){
if (u9!="MultiSelect"){
if (!(
(e4.allowColMove||e4.allowGroup)&&ac6.parentNode.getAttribute("FpSpread")=="ch"&&
u9=="Normal"&&(e4.getAttribute("SelectionPolicy")=="Range"||e4.getAttribute("SelectionPolicy")=="MultiRange")&&
e4.selectedCols.length!=0&&this.IsColSelected(e4,ac7)
))
this.ClearSelection(e4);
}
}else {
if (u9!="ExtendedSelect"&&u9!="MultiSelect"){
if (e4.d1!=null)this.PaintSelectedCell(e4,e4.d1,true);
}
}
}
e4.d1=ac6;
var h2=e4.d1;
var x2=this.GetParent(this.GetViewport(e4));
if (x2!=null&&!this.IsControl(n7)&&(n7!=null&&n7.tagName!="scrollbar")){
if (this.IsChild(h2,x2)&&h2.offsetLeft+h2.offsetWidth>x2.scrollLeft+x2.clientWidth){
x2.scrollLeft=h2.offsetLeft+h2.offsetWidth-x2.clientWidth;
}
if (this.IsChild(h2,x2)&&h2.offsetTop+h2.offsetHeight>x2.scrollTop+x2.clientHeight&&h2.offsetHeight<x2.clientHeight){
x2.scrollTop=h2.offsetTop+h2.offsetHeight-x2.clientHeight;
}
if (h2.offsetTop<x2.scrollTop){
x2.scrollTop=h2.offsetTop;
}
if (h2.offsetLeft<x2.scrollLeft){
x2.scrollLeft=h2.offsetLeft;
}
this.ScrollView(e4);
}
if (ac6.parentNode.getAttribute("FpSpread")!="ch")this.SetActiveRow(e4,this.GetRowKeyFromCell(e4,e4.d1));
if (ac6.parentNode.getAttribute("FpSpread")=="rh")
this.SetActiveCol(e4,0);
else {
this.SetActiveCol(e4,this.GetColKeyFromCell(e4,e4.d1));
}
var u9=this.GetOperationMode(e4);
if (e4.d1.parentNode.getAttribute("FpSpread")=="r"){
if (u9=="ExtendedSelect"||u9=="MultiSelect"){
var ac8=this.IsRowSelected(e4,this.GetRowFromCell(e4,e4.d1));
if (ac8)
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,false,true);
else 
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
}
else if (u9=="RowMode"||u9=="SingleSelect")
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
else {
this.SelectRange(e4,this.GetRowFromCell(e4,e4.d1),this.GetColFromCell(e4,e4.d1),1,1,true);
}
e4.d5=this.GetRowFromCell(e4,e4.d1);
e4.d6=this.GetColFromCell(e4,e4.d1);
}else if (e4.d1.parentNode.getAttribute("FpSpread")=="ch"){
if (n7.tagName=="INPUT"||n7.tagName=="TEXTAREA"||n7.tagName=="SELECT")
return ;
var r7=this.GetColFromCell(e4,e4.d1);
if (e4.allowColMove||e4.allowGroup)
{
if (u9=="Normal"&&(e4.getAttribute("SelectionPolicy")=="Range"||e4.getAttribute("SelectionPolicy")=="MultiRange")){
if (this.IsColSelected(e4,r7)){
this.InitMovingCol(e4,r7);
}else 
this.SelectColumn(e4,r7,1,true);
}
}else {
if (u9=="Normal"||u9=="ReadOnly"){
this.SelectColumn(e4,r7,1,true);
}
else 
return ;
}
}else if (e4.d1.parentNode.getAttribute("FpSpread")=="rh"){
if (n7.tagName=="INPUT"||n7.tagName=="TEXTAREA"||n7.tagName=="SELECT")
return ;
if (u9=="ExtendedSelect"||u9=="MultiSelect"){
if (this.IsRowSelected(e4,this.GetRowFromCell(e4,e4.d1)))
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,false,true);
else 
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
}else {
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true);
}
}
if (e4.d1!=null){
var g0=this.CreateEvent("ActiveCellChanged");
g0.cmdID=e4.id;
g0.Row=g0.row=this.GetSheetIndex(e4,this.GetRowFromCell(e4,e4.d1));
g0.Col=g0.col=this.GetColFromCell(e4,e4.d1);
if (e4.getAttribute("LayoutMode"))
g0.InnerRow=g0.innerRow=e4.d1.parentNode.getAttribute("row");
this.FireEvent(e4,g0);
}
e4.d2=e4.d1;
if (e4.d1!=null){
e4.d3=this.GetRowFromCell(e4,e4.d1);
e4.d4=this.GetColFromCell(e4,e4.d1);
}
this.b8=n7;
this.a6=true;
}
}
this.EnableButtons(e4);
if (!this.a7&&this.b5==null&&this.b4==null){
if (e4.d1!=null&&this.IsChild(e4.d1,e4)&&!this.IsHeaderCell(this.GetCell(n7))){
var i2=this.GetEditor(e4.d1);
if (i2!=null){
if (i2.type=="submit")this.SaveData(e4);
this.a7=(i2.type!="button"&&i2.type!="submit");
this.a8=i2;
this.a9=this.GetEditorValue(i2);
i2.focus();
}
}
}
if (!this.IsControl(n7)){
if (e4!=null)this.UpdatePostbackData(e4);
return this.CancelDefault(event);
}
}
this.GetMovingCol=function (e4){
var v6=document.getElementById(e4.id+"movingCol");
if (v6==null){
v6=document.createElement("DIV");
v6.style.display="none";
v6.style.position="absolute";
v6.style.top="0px";
v6.style.left="0px";
v6.id=e4.id+"movingCol";
v6.align="center";
e4.insertBefore(v6,null);
if (e4.getAttribute("DragColumnCssClass")!=null)
v6.className=e4.getAttribute("DragColumnCssClass");
else 
v6.style.border="1px solid black";
v6.style.MozOpacity=0.50;
}
return v6;
}
this.IsControl=function (f7){
return (f7!=null&&(f7.tagName=="INPUT"||f7.tagName=="TEXTAREA"||f7.tagName=="SELECT"||f7.tagName=="OPTION"));
}
this.EnableButtons=function (e4){
var r2=this.GetCellType(e4.d1);
var m8=this.GetSelection(e4);
var m9=m8.lastChild;
var s5=e4.getAttribute("OperationMode");
var ac9=s5=="ReadOnly"||s5=="SingleSelect"||r2=="readonly";
if (!ac9){
ac9=this.AnyReadOnlyCell(e4,m9);
}
if (ac9){
var f6=this.GetCmdBtn(e4,"Copy");
this.UpdateCmdBtnState(f6,m9==null);
var f3=this.b9;
f6=this.GetCmdBtn(e4,"Paste");
this.UpdateCmdBtnState(f6,(m9==null||f3==null));
f6=this.GetCmdBtn(e4,"Clear");
this.UpdateCmdBtnState(f6,true);
}else {
var f6=this.GetCmdBtn(e4,"Copy");
this.UpdateCmdBtnState(f6,m9==null);
var f3=this.b9;
f6=this.GetCmdBtn(e4,"Paste");
this.UpdateCmdBtnState(f6,(m9==null||f3==null));
f6=this.GetCmdBtn(e4,"Clear");
this.UpdateCmdBtnState(f6,m9==null);
}
}
this.CellClicked=function (h2){
var e4=this.GetSpread(h2);
if (e4!=null){
this.SaveData(e4);
}
}
this.UpdateCmdBtnState=function (f6,disabled){
if (f6==null)return ;
if (f6.tagName=="INPUT"){
var f7=f6.disabled;
if (f7==disabled)return ;
f6.disabled=disabled;
}else {
var f7=f6.getAttribute("disabled");
if (f7==disabled)return ;
f6.setAttribute("disabled",disabled);
}
if (f6.tagName=="IMG"){
var ad0=f6.getAttribute("disabledImg");
if (disabled&&ad0!=null&&ad0!=""){
if (f6.src.indexOf(ad0)<0)f6.src=ad0;
}else {
var ad1=f6.getAttribute("enabledImg");
if (f6.src.indexOf(ad1)<0)f6.src=ad1;
}
}
}
this.MouseUp=function (event){
if (window.fpPostOn!=null)return ;
event=this.GetEvent(event);
var n7=this.GetTarget(event);
var e4=this.GetSpread(n7,true);
if (e4==null&&!this.a6){
return ;
}
if (this.dragSlideBar&&e4!=null)
{
this.dragSlideBar=false;
if (this.activePager!=null){
var ab3=this.MoveSliderBar(e4,event)-1;
this.activePager=null;
this.GotoPage(e4,ab3);
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
var s5=this.GetOperationMode(e4);
if (s5=="ReadOnly")return ;
var i1=true;
if (this.a6){
this.a6=false;
if (this.dragCol!=null&&this.dragCol>=0){
var ad2=(this.IsChild(n7,this.GetGroupBar(e4))||n7==this.GetGroupBar(e4));
if (!ad2&&this.GetGroupBar(e4)!=null){
var ad3=event.clientX;
var ad4=event.clientY;
var p8=e4.offsetLeft;
var u0=e4.offsetTop;
var ad5=this.GetGroupBar(e4).offsetWidth;
var ad6=this.GetGroupBar(e4).offsetHeight;
var p3=window.scrollX;
var p2=window.scrollY;
var k8=document.getElementById(e4.id+"_titleBar");
if (k8)p2-=k8.parentNode.parentNode.offsetHeight;
if (this.GetPager1(e4)!=null)p2-=this.GetPager1(e4).offsetHeight;
ad2=(p8<=p3+ad3&&p3+ad3<=p8+ad5&&u0<=p2+ad4&&p2+ad4<=u0+ad6);
}
if (e4.dragFromGroupbar){
if (ad2){
if (e4.targetCol>0)
this.Regroup(e4,this.dragCol,parseInt((e4.targetCol+1)/2));
else 
this.Regroup(e4,this.dragCol,e4.targetCol);
}else {
this.Ungroup(e4,this.dragCol,e4.targetCol);
}
}else {
if (ad2){
if (e4.allowGroup){
if (e4.targetCol>0)
this.Group(e4,this.dragCol,parseInt((e4.targetCol+1)/2));
else 
this.Group(e4,this.dragCol,e4.targetCol);
}
}else if (e4.allowColMove){
if (e4.targetCol!=null){
var g0=this.CreateEvent("ColumnDragMove");
g0.cancel=false;
g0.col=e4.selectedCols;
this.FireEvent(e4,g0);
if (!g0.cancel){
this.MoveCol(e4,this.dragCol,e4.targetCol);
var g0=this.CreateEvent("ColumnDragMoveCompleted");
g0.col=e4.selectedCols;
this.FireEvent(e4,g0);
}
}
}
}
var v6=this.GetMovingCol(e4);
if (v6!=null)
v6.style.display="none";
this.dragCol=-1;
this.dragViewCol=-1;
var ab4=this.GetPosIndicator(e4);
if (ab4!=null)
ab4.style.display="none";
e4.dragFromGroupbar=false;
e4.targetCol=null;
this.b4=this.b5=null;
}
if (this.b4!=null){
i1=false;
var ac1=event.clientX-this.b6;
var w6=parseInt(this.b4.width);
var ad7=w6;
if (isNaN(w6))w6=0;
w6+=ac1;
if (w6<1)w6=1;
var k3=parseInt(this.b4.getAttribute("index"));
var ad8=this.GetColGroup(this.GetViewport(e4));
if (ad8!=null&&ad8.childNodes.length>0){
ad7=parseInt(ad8.childNodes[k3].width);
}else {
ad7=1;
}
if (this.GetViewport(e4).rules!="rows"){
if (k3==0)ad7+=1;
if (k3==parseInt(this.colCount)-1)ad7-=1;
}
if (w6!=ad7&&event.clientX!=this.b7)
{
this.SetColWidth(e4,k3,w6);
var g0=this.CreateEvent("ColWidthChanged");
g0.col=k3;
g0.width=w6;
this.FireEvent(e4,g0);
}
this.ScrollView(e4);
this.PaintFocusRect(e4);
}else if (this.b5!=null){
i1=false;
var ac1=event.clientY-this.b7;
var ac3=this.b5.offsetHeight+ac1;
if (ac3<1){
ac3=1;
ac1=1-this.b5.offsetHeight;
}
this.b5.cells[0].style.posHeight=this.b5.cells[1].style.posHeight=ac3;
this.b5.style.cursor="auto";
var i3=null;
i3=this.GetViewport(e4);
if (typeof(i3.rows[this.b5.rowIndex])!="undefined"&&
typeof(i3.rows[this.b5.rowIndex].cells[0])!="undefined")
{
i3.rows[this.b5.rowIndex].cells[0].style.height=this.b5.cells[0].style.height;
}
var p7=this.AddRowInfo(e4,this.b5.getAttribute("FpKey"));
if (p7!=null){
if (this.b5.cells[0])
this.SetRowHeight(e4,p7,parseInt(this.b5.cells[0].style.posHeight));
else 
this.SetRowHeight(e4,p7,parseInt(this.b5.style.height));
}
if (this.b6!=event.clientY){
var g0=this.CreateEvent("RowHeightChanged");
g0.row=this.GetRowFromCell(e4,this.b5.cells[0]);
g0.height=this.b5.offsetHeight;
this.FireEvent(e4,g0);
}
var i4=this.GetParentSpread(e4);
if (i4!=null)this.UpdateRowHeight(i4,e4);
var e7=this.GetTopSpread(e4);
this.SizeAll(e7);
this.Refresh(e7);
this.ScrollView(e4);
this.PaintFocusRect(e4);
}else {
}
if (this.b8!=null){
this.b8=null;
}
}
if (i1)i1=!this.IsControl(n7);
if (i1&&this.HitCommandBar(n7))return ;
var ad9=false;
var m8=this.GetSelection(e4);
if (m8!=null){
var m9=m8.firstChild;
var h7=new this.Range();
if (m9!=null){
h7.row=this.GetRowByKey(e4,m9.getAttribute("row"));
h7.col=this.GetColByKey(e4,m9.getAttribute("col"));
h7.rowCount=parseInt(m9.getAttribute("rowcount"));
h7.colCount=parseInt(m9.getAttribute("colcount"));
}
switch (e4.d7){
case "":
var g7=this.GetViewport(e4).rows;
for (var e9=h7.row;e9<h7.row+h7.rowCount&&e9<g7.length;e9++){
if (g7[e9].cells.length>0&&g7[e9].cells[0].firstChild!=null&&g7[e9].cells[0].firstChild.nodeName!="#text"){
if (g7[e9].cells[0].firstChild.getAttribute("FpSpread")=="Spread"){
ad9=true;
break ;
}
}
}
break ;
case "c":
var i3=this.GetViewport(e4);
for (var e9=0;e9<i3.rows.length;e9++){
if (this.IsChildSpreadRow(e4,i3,e9)){
ad9=true;
break ;
}
}
break ;
case "r":
var i3=this.GetViewport(e4);
var v2=h7.rowCount;
for (var e9=h7.row;e9<h7.row+v2&&e9<i3.rows.length;e9++){
if (this.IsChildSpreadRow(e4,i3,e9)){
ad9=true;
break ;
}
}
}
}
if (ad9){
var f6=this.GetCmdBtn(e4,"Copy");
this.UpdateCmdBtnState(f6,true);
f6=this.GetCmdBtn(e4,"Paste");
this.UpdateCmdBtnState(f6,true);
f6=this.GetCmdBtn(e4,"Clear");
this.UpdateCmdBtnState(f6,true);
}
var j8=this.IsXHTML(e4);
if (j8){
var e7=this.GetTopSpread(e4);
var f8=document.getElementById(e7.id+"_textBox");
if (f8!=null){
f8.style.top=event.clientY-e4.offsetTop;
f8.style.left=event.clientX-e4.offsetLeft;
}
}
if (i1)this.Focus(e4);
}
this.UpdateRowHeight=function (i4,child){
var l9=child.parentNode;
while (l9!=null){
if (l9.tagName=="TR")break ;
l9=l9.parentNode;
}
var j8=this.IsXHTML(i4);
if (l9!=null){
var e8=l9.rowIndex;
if (this.GetRowHeader(i4)!=null){
var p5=0;
if (this.GetColHeader(child)!=null)p5=this.GetColHeader(child).offsetHeight;
if (this.GetRowHeader(child)!=null)p5+=this.GetRowHeader(child).offsetHeight;
if (!j8)p5-=this.GetViewport(i4).cellSpacing;
if (this.GetViewport(i4).cellSpacing==0){
this.GetRowHeader(i4).rows[e8].cells[0].style.posHeight=p5;
if (this.GetParentSpread(i4)!=null){
this.GetRowHeader(i4).parentNode.style.posHeight=this.GetRowHeader(i4).offsetHeight;
}
}
else 
this.GetRowHeader(i4).rows[e8].cells[0].style.posHeight=(p5+2);
this.GetViewport(i4).rows[e8].cells[0].style.posHeight=p5;
if (!j8)p5-=1;
child.style.posHeight=p5;
}
}
var ae0=this.GetParentSpread(i4);
if (ae0!=null)
this.UpdateRowHeight(ae0,i4);
}
this.MouseOut=function (){
if (!this.a6&&this.b4!=null&&this.b4.style!=null)this.b4.style.cursor="auto";
}
this.KeyDown=function (e4,event){
if (window.fpPostOn!=null)return ;
if (!e4.ProcessKeyMap(event))return ;
if (event.keyCode==this.space&&e4.d1!=null){
var u9=this.GetOperationMode(e4);
if (u9=="MultiSelect"){
if (this.IsRowSelected(e4,this.GetRowFromCell(e4,e4.d1)))
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,false,true);
else 
this.SelectRow(e4,this.GetRowFromCell(e4,e4.d1),1,true,true);
return ;
}
}
var i2=false;
if (this.a7&&this.a8!=null){
var ae1=this.GetEditor(this.a8);
i2=(ae1!=null);
}
if (event.keyCode!=this.enter&&event.keyCode!=this.tab&&(this.a7&&!i2)&&this.a8.tagName=="SELECT")return ;
switch (event.keyCode){
case this.left:
case this.right:
if (i2){
var ae2=this.a8.getAttribute("FpEditor");
if (this.a7&&ae2=="ExtenderEditor"){
var ae3=FpExtender.Util.getEditor(this.a8);
if (ae3&&ae3.type!="text")this.EndEdit();
}
if (ae2!="RadioButton"&&ae2!="ExtenderEditor")this.EndEdit();
}
if (!this.a7){
this.NextCell(e4,event,event.keyCode);
}
break ;
case this.up:
case this.down:
case this.enter:
if (this.a8!=null&&this.a8.tagName=="TEXTAREA")return ;
if (/*event.keyCode!=event.DOM_VK_RETURN&&*/i2&&this.a7&&this.a8.getAttribute("FpEditor")=="ExtenderEditor"){
var ae4=this.a8.getAttribute("Extenders");
if (ae4&&ae4.indexOf("AutoCompleteExtender")!=-1)return ;
}
if (event.keyCode==event.DOM_VK_RETURN)this.CancelDefault(event);
if (this.a7){
var o0=this.EndEdit();
if (!o0)return ;
}
this.NextCell(e4,event,event.keyCode);
var e7=this.GetTopSpread(e4);
var f8=document.getElementById(e7.id+"_textBox");
if (this.enter==event.keyCode)f8.focus();
break ;
case this.tab:
if (this.a7){
var o0=this.EndEdit();
if (!o0)return ;
}
var n9=this.GetProcessTab(e4);
var ae5=(n9=="true"||n9=="True");
if (ae5)this.NextCell(e4,event,event.keyCode);
break ;
case this.shift:
break ;
case this.home:
case this.end:
case this.pup:
case this.pdn:
this.CancelDefault(event);
if (!this.a7){
this.NextCell(e4,event,event.keyCode);
}
break ;
default :
if (event.keyCode==67&&event.ctrlKey&&(!this.a7||i2))this.Copy(e4);
else if (event.keyCode==86&&event.ctrlKey&&(!this.a7||i2))this.Paste(e4);
else if (event.keyCode==88&&event.ctrlKey&&(!this.a7||i2))this.Clear(e4);
else if (!this.a7&&e4.d1!=null&&!this.IsHeaderCell(e4.d1)&&!event.ctrlKey&&!event.altKey){
this.StartEdit(e4,e4.d1);
}
break ;
}
}
this.GetProcessTab=function (e4){
return e4.getAttribute("ProcessTab");
}
this.ExpandRow=function (e4,i5){
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"ExpandView,"+i5,e4);
else 
__doPostBack(s3,"ExpandView,"+i5);
}
this.SortColumn=function (e4,column){
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"SortColumn,"+column,e4);
else 
__doPostBack(s3,"SortColumn,"+column);
}
this.Filter=function (event,e4){
var n7=this.GetTarget(event);
var f7=n7.value;
if (n7.tagName=="SELECT"){
var y4=new RegExp("\\s*");
var ae6=new RegExp("\\S*");
var s6=n7[n7.selectedIndex].text;
var ae7="";
var e9=0;
var e8=f7.length;
while (e8>0){
var h3=f7.match(y4);
if (h3!=null){
ae7+=h3[0];
e9=h3[0].length;
e8-=e9;
f7=f7.substring(e9);
h3=f7.match(ae6);
if (h3!=null){
e9=h3[0].length;
e8-=e9;
f7=f7.substring(e9);
}
}else {
break ;
}
h3=s6.match(ae6);
if (h3!=null){
ae7+=h3[0];
e9=h3[0].length;
s6=s6.substring(e9);
h3=s6.match(y4);
if (h3!=null){
e9=h3[0].length;
s6=s6.substring(e9);
}
}else {
break ;
}
}
f7=ae7;
}
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(n7.name,f7,e4);
else 
__doPostBack(n7.name,f7);
}
this.MoveCol=function (e4,from,to){
var s3=e4.getAttribute("name");
if (e4.selectedCols&&e4.selectedCols.length>0){
var ae8=[];
for (var e9=0;e9<e4.selectedCols.length;e9++)
ae8[e9]=this.GetSheetColIndex(e4,e4.selectedCols[e9]);
var ae9=ae8.join("+");
this.MoveMultiCol(e4,ae9,to);
return ;
}
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"MoveCol,"+from+","+to,e4);
else 
__doPostBack(s3,"MoveCol,"+from+","+to);
}
this.MoveMultiCol=function (e4,ae9,to){
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"MoveCol,"+ae9+","+to,e4);
else 
__doPostBack(s3,"MoveCol,"+ae9+","+to);
}
this.Group=function (e4,m7,toCol){
var s3=e4.getAttribute("name");
if (e4.selectedCols&&e4.selectedCols.length>0){
var ae8=[];
for (var e9=0;e9<e4.selectedCols.length;e9++)
ae8[e9]=this.GetSheetColIndex(e4,e4.selectedCols[e9]);
var ae9=ae8.join("+");
this.GroupMultiCol(e4,ae9,toCol);
e4.selectedCols=[];
return ;
}
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Group,"+m7+","+toCol,e4);
else 
__doPostBack(s3,"Group,"+m7+","+toCol);
}
this.GroupMultiCol=function (e4,ae9,toCol){
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Group,"+ae9+","+toCol,e4);
else 
__doPostBack(s3,"Group,"+ae9+","+toCol);
}
this.Ungroup=function (e4,m7,toCol){
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Ungroup,"+m7+","+toCol,e4);
else 
__doPostBack(s3,"Ungroup,"+m7+","+toCol);
}
this.Regroup=function (e4,fromCol,toCol){
var s3=e4.getAttribute("name");
var y5=(e4.getAttribute("ajax")!="false");
if (y5)
this.SyncData(s3,"Regroup,"+fromCol+","+toCol,e4);
else 
__doPostBack(s3,"Regroup,"+fromCol+","+toCol);
}
this.ProcessData=function (){
try {
var af0=this;
af0.removeEventListener("load",the_fpSpread.ProcessData,false);
var n7=window.srcfpspread;
n7=n7.split(":").join("_");
var af1=window.fpcommand;
var af2=document;
var af3=af2.getElementById(n7+"_buff");
if (af3==null){
af3=af2.createElement("iframe");
af3.id=n7+"_buff";
af3.style.display="none";
af2.body.appendChild(af3);
}
var e4=af2.getElementById(n7);
the_fpSpread.CloseWaitMsg(e4);
if (af3==null)return ;
var af4=af0.responseText;
af3.contentWindow.document.body.innerHTML=af4;
var n9=af3.contentWindow.document.getElementById(n7+"_values");
if (n9!=null){
var s2=n9.getElementsByTagName("data")[0];
var m9=s2.firstChild;
the_fpSpread.error=false;
while (m9!=null){
var g9=the_fpSpread.GetRowByKey(e4,m9.getAttribute("r"));
var h1=the_fpSpread.GetColByKey(e4,m9.getAttribute("c"));
var y7=the_fpSpread.GetValue(e4,g9,h1);
if (m9.innerHTML!=y7){
var i1=the_fpSpread.GetFormula(e4,g9,h1);
var i8=the_fpSpread.GetCellByRowCol(e4,g9,h1);
the_fpSpread.SetCellValueFromView(i8,m9.innerHTML,true);
i8.setAttribute("FpFormula",i1);
}
m9=m9.nextSibling;
}
the_fpSpread.ClearCellData(e4);
}else {
the_fpSpread.UpdateSpread(af2,af3,n7,af4,af1);
}
var y6=af2.getElementsByTagName("FORM");
y6[0].__EVENTTARGET.value="";
y6[0].__EVENTARGUMENT.value="";
var y7=af2.getElementsByName("__VIEWSTATE")[0];
var f7=af3.contentWindow.document.getElementsByName("__VIEWSTATE")[0];
if (y7!=null&&f7!=null)y7.value=f7.value;
y7=af2.getElementsByName("__EVENTVALIDATION");
f7=af3.contentWindow.document.getElementsByName("__EVENTVALIDATION");
if (y7!=null&&f7!=null&&y7.length>0&&f7.length>0)
y7[0].value=f7[0].value;
af3.contentWindow.document.location="about:blank";
window.fpPostOn=null;
d8=null;
}catch (g0){
window.fpPostOn=null;
d8=null;
}
var e4=the_fpSpread.GetTopSpread(af2.getElementById(n7));
var g0=the_fpSpread.CreateEvent("CallBackStopped");
g0.command=af1;
the_fpSpread.FireEvent(e4,g0);
};
this.UpdateSpread=function (af2,af3,n7,af4,af1){
var e4=the_fpSpread.GetTopSpread(af2.getElementById(n7));
var r3=af3.contentWindow.document.getElementById(e4.id);
if (r3!=null){
if (typeof(Sys)!=='undefined'){
FarPoint.System.ExtenderHelper.saveLoadedExtenderScripts(e4);
}
the_fpSpread.error=(r3.getAttribute("error")=="true");
if (af1=="LoadOnDemand"&&!the_fpSpread.error){
var af5=this.GetElementById(e4,e4.id+"_data");
var af6=this.GetElementById(r3,e4.id+"_data");
if (af5!=null&&af6!=null)af5.setAttribute("data",af6.getAttribute("data"));
var af7=r3.getElementsByTagName("style");
if (af7!=null){
for (var e9=0;e9<af7.length;e9++){
if (af7[e9]!=null&&af7[e9].innerHTML!=null&&af7[e9].innerHTML.indexOf(e4.id+"msgStyle")<0)
e4.appendChild(af7[e9].cloneNode(true));
}
}
var af8=this.GetElementById(e4,e4.id+"_LoadInfo");
var af9=this.GetElementById(r3,e4.id+"_LoadInfo");
if (af8!=null&&af9!=null)af8.value=af9.value;
var ag0=false;
var ag1=this.GetElementById(r3,e4.id+"_rowHeader");
if (ag1!=null){
ag1=ag1.firstChild;
ag0=(ag1.rows.length>1);
var j4=this.GetRowHeader(e4);
this.LoadRows(j4,ag1,true);
}
var ag2=this.GetElementById(r3,e4.id+"_viewport");
if (ag2!=null){
ag0=(ag2.rows.length>0);
var e5=this.GetViewport(e4);
this.LoadRows(e5,ag2,false);
}
the_fpSpread.Init(e4);
the_fpSpread.LoadScrollbarState(e4);
the_fpSpread.Focus(e4);
if (ag0)
e4.LoadState=null;
else 
e4.LoadState="complete";
if (typeof(Sys)!=='undefined'){
FarPoint.System.ExtenderHelper.loadExtenderScripts(e4,af3.contentWindow.document);
}
}else {
e4.innerHTML=r3.innerHTML;
the_fpSpread.CopySpreadAttrs(r3,e4);
if (typeof(Sys)!=='undefined'){
FarPoint.System.ExtenderHelper.loadExtenderScripts(e4,af3.contentWindow.document);
}
var ag3=af3.contentWindow.document.getElementById(e4.id+"_initScript");
eval(ag3.value);
for (var e9=0;e9<af3.contentWindow.document.styleSheets.length;e9++){
for (var h9=0;h9<af3.contentWindow.document.styleSheets[e9].rules.length;h9++){
var ag4=af3.contentWindow.document.styleSheets[e9].rules[h9];
var ag5={styleSheetIndex:-1,ruleIndex:-1};
for (var ag6=0;ag6<af2.styleSheets.length;ag6++){
for (var ag7=0;ag7<af2.styleSheets[ag6].rules.length;ag7++){
if (af2.styleSheets[ag6].rules[ag7].selectorText==ag4.selectorText){
ag5.styleSheetIndex=ag6;
ag5.ruleIndex=ag7;
}
}
}
if (ag5.styleSheetIndex>-1&&ag5.ruleIndex>-1)
af2.styleSheets[ag5.styleSheetIndex].deleteRule(ag5.ruleIndex);
af2.styleSheets[0].addRule(ag4.selectorText,ag4.style.cssText);
}
}
}
}else {
the_fpSpread.error=true;
}
}
this.LoadRows=function (e5,ag2,isHeader){
if (e5==null||ag2==null)return ;
var ag8=e5.tBodies[0];
var v2=ag2.rows.length;
var ag9=null;
if (isHeader){
v2--;
if (ag8.rows.length>0)ag9=ag8.rows[ag8.rows.length-1];
}
for (var e9=0;e9<v2;e9++){
var ah0=ag2.rows[e9].cloneNode(false);
ag8.insertBefore(ah0,ag9);
ah0.innerHTML=ag2.rows[e9].innerHTML;
}
if (!isHeader){
for (var e9=0;e9<ag2.parentNode.childNodes.length;e9++){
var y1=ag2.parentNode.childNodes[e9];
if (y1!=ag2){
e5.parentNode.insertBefore(y1.cloneNode(true),null);
}
}
}
}
this.CopySpreadAttr=function (aa2,dest,attrName){
var ah1=aa2.getAttribute(attrName);
var ah2=dest.getAttribute(attrName);
if (ah1!=null||ah2!=null){
if (ah1==null)
dest.removeAttribute(attrName);
else 
dest.setAttribute(attrName,ah1);
}
}
this.CopySpreadAttrs=function (aa2,dest){
this.CopySpreadAttr(aa2,dest,"totalRowCount");
this.CopySpreadAttr(aa2,dest,"pageCount");
this.CopySpreadAttr(aa2,dest,"loadOnDemand");
this.CopySpreadAttr(aa2,dest,"allowGroup");
this.CopySpreadAttr(aa2,dest,"colMove");
this.CopySpreadAttr(aa2,dest,"showFocusRect");
this.CopySpreadAttr(aa2,dest,"FocusBorderColor");
this.CopySpreadAttr(aa2,dest,"FocusBorderStyle");
this.CopySpreadAttr(aa2,dest,"FpDefaultEditorID");
this.CopySpreadAttr(aa2,dest,"hierView");
this.CopySpreadAttr(aa2,dest,"IsNewRow");
this.CopySpreadAttr(aa2,dest,"cmdTop");
this.CopySpreadAttr(aa2,dest,"ProcessTab");
this.CopySpreadAttr(aa2,dest,"AcceptFormula");
this.CopySpreadAttr(aa2,dest,"EditMode");
this.CopySpreadAttr(aa2,dest,"AllowInsert");
this.CopySpreadAttr(aa2,dest,"AllowDelete");
this.CopySpreadAttr(aa2,dest,"error");
this.CopySpreadAttr(aa2,dest,"ajax");
this.CopySpreadAttr(aa2,dest,"autoCalc");
this.CopySpreadAttr(aa2,dest,"multiRange");
this.CopySpreadAttr(aa2,dest,"rowFilter");
this.CopySpreadAttr(aa2,dest,"OperationMode");
this.CopySpreadAttr(aa2,dest,"selectedForeColor");
this.CopySpreadAttr(aa2,dest,"selectedBackColor");
this.CopySpreadAttr(aa2,dest,"anchorBackColor");
this.CopySpreadAttr(aa2,dest,"columnHeaderAutoTextIndex");
this.CopySpreadAttr(aa2,dest,"SelectionPolicy");
this.CopySpreadAttr(aa2,dest,"ShowHeaderSelection");
this.CopySpreadAttr(aa2,dest,"EnableRowEditTemplate");
this.CopySpreadAttr(aa2,dest,"scrollContent");
this.CopySpreadAttr(aa2,dest,"scrollContentColumns");
this.CopySpreadAttr(aa2,dest,"scrollContentTime");
this.CopySpreadAttr(aa2,dest,"scrollContentMaxHeight");
dest.tabIndex=aa2.tabIndex;
if (dest.style!=null&&aa2.style!=null){
if (dest.style.width!=aa2.style.width)dest.style.width=aa2.style.width;
if (dest.style.height!=aa2.style.height)dest.style.height=aa2.style.height;
if (dest.style.border!=aa2.style.border)dest.style.border=aa2.style.border;
}
}
this.Clone=function (l7){
var f7=document.createElement(l7.tagName);
f7.id=l7.id;
var h1=l7.firstChild;
while (h1!=null){
var j6=this.Clone(h1);
f7.appendChild(j6);
h1=h1.nextSibling;
}
return f7;
}
this.FireEvent=function (e4,g0){
if (e4==null||g0==null)return ;
var e7=this.GetTopSpread(e4);
if (e7!=null){
g0.spread=e4;
e7.dispatchEvent(g0);
}
}
this.SyncData=function (s3,af1,e4,asyncCallBack){
if (window.fpPostOn!=null){
return ;
}
var g0=this.CreateEvent("CallBackStart");
g0.cancel=false;
g0.command=af1;
if (asyncCallBack==null)asyncCallBack=false;
g0.async=asyncCallBack;
if (e4==null){
var j6=s3.split(":").join("_");
e4=document.getElementById(j6);
}
if (e4!=null){
var e7=this.GetTopSpread(e4);
this.FireEvent(e4,g0);
}
if (g0.cancel){
the_fpSpread.ClearCellData(e4);
return ;
}
if (af1!=null&&(af1.indexOf("SelectView,")==0||af1=="Next"||af1=="Prev"||af1.indexOf("Group,")==0||af1.indexOf("Page,")==0))
e4.LoadState=null;
var ah3=g0.async;
if (ah3){
this.OpenWaitMsg(e4);
}
window.fpPostOn=true;
if (this.error)af1="update";
try {
var y6=document.getElementsByTagName("FORM");
if (y6==null&&y6.length==0)return ;
y6[0].__EVENTTARGET.value=s3;
y6[0].__EVENTARGUMENT.value=encodeURIComponent(af1);
var ah4=y6[0].action;
var f7;
if (ah4.indexOf("?")>-1){
f7="&";
}
else 
{
f7="?";
}
ah4=ah4+f7;
var f3=this.CollectData();
var af4="";
var af0=(window.XMLHttpRequest)?new XMLHttpRequest():new ActiveXObject("Microsoft.XMLHTTP");
if (af0==null)return ;
af0.open("POST",ah4,ah3);
af0.setRequestHeader("Content-Type","application/x-www-form-urlencoded");
if (e4!=null)
window.srcfpspread=e4.id;
else 
window.srcfpspread=s3;
window.fpcommand=af1;
this.AttachEvent(af0,"load",the_fpSpread.ProcessData,false);
af0.send(f3);
}catch (g0){
window.fpPostOn=false;
d8=null;
}
};
this.CollectData=function (){
var y6=document.getElementsByTagName("FORM");
var f7;
var g2="fpcallback=true&";
for (var e9=0;e9<y6[0].elements.length;e9++){
f7=y6[0].elements[e9];
var ah5=f7.tagName.toLowerCase();
if (ah5=="input"){
var ah6=f7.type;
if (ah6=="hidden"||ah6=="text"||ah6=="password"||((ah6=="checkbox"||ah6=="radio")&&f7.checked)){
g2+=(f7.name+"="+encodeURIComponent(f7.value)+"&");
}
}else if (ah5=="select"){
if (f7.childNodes!=null){
for (var h9=0;h9<f7.childNodes.length;h9++){
var p6=f7.childNodes[h9];
if (p6!=null&&p6.tagName!=null&&p6.tagName.toLowerCase()=="option"&&p6.selected){
g2+=(f7.name+"="+encodeURIComponent(p6.value)+"&");
}
}
}
}else if (ah5=="textarea"){
g2+=(f7.name+"="+encodeURIComponent(f7.value)+"&");
}
}
return g2;
};
this.ClearCellData=function (e4){
var f3=this.GetData(e4);
var ah7=f3.getElementsByTagName("root")[0];
var f4=ah7.getElementsByTagName("data")[0];
if (f4==null)return null;
if (e4.d8!=null){
var i5=e4.d8.firstChild;
while (i5!=null){
var g9=i5.getAttribute("key");
var ah8=i5.firstChild;
while (ah8!=null){
var h1=ah8.getAttribute("key");
var ah9=f4.firstChild;
while (ah9!=null){
var h3=ah9.getAttribute("key");
if (g9==h3){
var ai0=false;
var ai1=ah9.firstChild;
while (ai1!=null){
var h4=ai1.getAttribute("key");
if (h1==h4){
ah9.removeChild(ai1);
ai0=true;
break ;
}
ai1=ai1.nextSibling;
}
if (ai0)break ;
}
ah9=ah9.nextSibling;
}
ah8=ah8.nextSibling;
}
i5=i5.nextSibling;
}
}
e4.d8=null;
var f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null)
this.UpdateCmdBtnState(f6,true);
}
this.StorePostData=function (e4){
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
var ab1=f4.getElementsByTagName("data")[0];
if (ab1!=null)e4.d8=ab1.cloneNode(true);
}
this.ShowMessage=function (e4,u5,i5,m7,time){
var n0=e4.GetRowCount();
var g8=e4.GetColCount();
if (i5==null||m7==null||i5<0||i5>=n0||m7<0||m7>=g8){
i5=-1;
m7=-1;
}
this.ShowMessageInner(e4,u5,i5,m7,time);
}
this.HideMessage=function (e4,i5,m7){
var n0=e4.GetRowCount();
var g8=e4.GetColCount();
if (i5==null||m7==null||i5<0||i5>=n0||m7<0||m7>=g8)
if (e4.msgList&&e4.msgList.centerMsg&&e4.msgList.centerMsg.msgBox.IsVisible)
e4.msgList.centerMsg.msgBox.Hide();
var ai2=this.GetMsgObj(e4,i5,m7);
if (ai2&&ai2.msgBox.IsVisible){
ai2.msgBox.Hide();
}
}
this.ShowMessageInner=function (e4,u5,i5,m7,time){
var ai2=this.GetMsgObj(e4,i5,m7);
if (ai2){
if (ai2.timer)
ai2.msgBox.Hide();
}
else 
ai2=this.CreateMsgObj(e4,i5,m7);
var ai3=ai2.msgBox;
ai3.Show(e4,this,u5);
if (time&&time>0)
ai2.timer=setTimeout(function (){ai3.Hide();},time);
this.SetMsgObj(e4,ai2);
}
this.GetMsgObj=function (e4,i5,m7){
var ai2;
var ai4=e4.msgList;
if (ai4){
if (i5==-1&&m7==-1)
ai2=ai4.centerMsg;
else if (i5==-2)
ai2=ai4.hScrollMsg;
else if (m7==-2)
ai2=ai4.vScrollMsg;
else {
if (ai4[i5])
ai2=ai4[i5][m7];
}
}
return ai2;
}
this.SetMsgObj=function (e4,ai2){
var ai4=e4.msgList;
if (ai2.row==-1&&ai2.col==-1)
ai4.centerMsg=ai2;
else if (ai2.row==-2)
ai4.hScrollMsg=ai2;
else if (ai2.col==-2)
ai4.vScrollMsg=ai2;
else {
if (!ai4[ai2.row])ai4[ai2.row]=new Array();
ai4[ai2.row][ai2.col]=ai2;
}
}
var ai5=null;
this.CreateMsgObj=function (e4,i5,m7){
var ai3=document.createElement("div");
var ai2={row:i5,col:m7,msgBox:ai3};
var ai6=null;
if (i5!=-2&&m7!=-2){
ai3.style.border="1px solid black";
ai3.style.background="yellow";
ai3.style.color="red";
}
else {
ai3.style.border="1px solid #55678e";
ai3.style.fontSize="small";
ai3.style.background="#E6E9ED";
ai3.style.color="#4c5b7f";
this.GetScrollingContentStyle(e4);
ai6=ai5;
}
if (ai6!=null){
if (ai6.fontFamily!=null)
ai3.style.fontFamily=ai6.fontFamily;
if (ai6.fontSize!=null)
ai3.style.fontSize=ai6.fontSize;
if (ai6.fontStyle!=null)
ai3.style.fontStyle=ai6.fontStyle;
if (ai6.fontVariant!=null)
ai3.style.fontVariant=ai6.fontVariant;
if (ai6.fontWeight!=null)
ai3.style.fontWeight=ai6.fontWeight;
if (ai6.backgroundColor!=null)
ai3.style.backgroundColor=ai6.backgroundColor;
if (ai6.color!=null)
ai3.style.color=ai6.color;
}
ai3.style.position="absolute";
ai3.style.overflow="hidden";
ai3.style.display="block";
ai3.style.marginLeft=0;
ai3.style.marginTop=2;
ai3.style.marginRight=0;
ai3.style.marginBottom=0;
ai3.msgObj=ai2;
ai3.Show=function (r3,fpObj,u5){
var v4=fpObj.GetMsgPos(r3,this.msgObj.row,this.msgObj.col);
var e6=fpObj.GetCommandBar(r3);
var ai7=fpObj.GetGroupBar(r3);
this.style.visibility="visible";
this.style.display="block";
if (u5){
this.style.left=""+0+"px";
this.style.top=""+0+"px";
this.style.width="auto";
this.innerHTML=u5;
}
var ai8=false;
var ai9=(r3.style.position=="relative"||r3.style.position=="absolute");
var aj0=v4.top;
var aj1=v4.left;
var p6=e4.offsetParent;
while ((p6.tagName=="TD"||p6.tagName=="TR"||p6.tagName=="TBODY"||p6.tagName=="TABLE")&&p6.style.position!="relative"&&p6.style.position!="absolute")
p6=p6.offsetParent;
if (this.msgObj.row>=0&&this.msgObj.col>=0){
if (!ai9&&ai8&&p6){
var aj2=fpObj.GetLocation(r3);
var aj3=fpObj.GetLocation(p6);
aj0+=aj2.y-aj3.y;
aj1+=aj2.x-aj3.x;
if (p6.tagName!=="BODY"){
aj0-=fpObj.GetBorderWidth(p6,0);
aj1-=fpObj.GetBorderWidth(p6,3);
}
}
var aj4=fpObj.GetViewPortByRowCol(r3,this.msgObj.row,this.msgObj.col);
if (!this.parentNode&&aj4&&aj4.parentNode)aj4.parentNode.insertBefore(ai3,null);
var i9=this.offsetWidth;
this.style.left=""+aj1+"px";
if (!ai8&&aj4&&aj4.parentNode&&aj1+i9>aj4.offsetWidth)
this.style.width=""+(v4.a5-2)+"px";
else if (parseInt(this.style.width)!=i9)
this.style.width=""+i9+"px";
if (!ai8&&aj4!=null&&aj0>=aj4.offsetHeight-2)aj0-=v4.a4+this.offsetHeight+3;
this.style.top=""+aj0+"px";
}
else {
if (!ai9&&p6){
var aj2=fpObj.GetLocation(r3);
var aj3=fpObj.GetLocation(p6);
aj0+=aj2.y-aj3.y;
aj1+=aj2.x-aj3.x;
if (p6.tagName!=="BODY"){
aj0-=fpObj.GetBorderWidth(p6,0);
aj1-=fpObj.GetBorderWidth(p6,3);
}
}
var aj5=20;
if (!this.parentNode)r3.insertBefore(ai3,null);
if (this.offsetWidth+aj5<r3.offsetWidth)
aj1+=(r3.offsetWidth-this.offsetWidth-aj5)/(this.msgObj.row==-2?1:2);
else 
this.style.width=""+(r3.offsetWidth-aj5)+"px";
if (this.offsetHeight<r3.offsetHeight)
aj0+=(r3.offsetHeight-this.offsetHeight)/(this.msgObj.col==-2?1:2);
if (this.msgObj.col==-2){
var aj6=fpObj.GetColFooter(r3);
if (aj6)aj0-=aj6.offsetHeight;
var e6=fpObj.GetCommandBar(r3);
if (e6)aj0-=e6.offsetHeight;
aj0-=aj5;
}
this.style.top=""+aj0+"px";
this.style.left=""+aj1+"px";
}
this.IsVisible=true;
};
ai3.Hide=function (){
this.style.visibility="hidden";
this.style.display="none";
this.IsVisible=false;
if (this.msgObj.timer){
clearTimeout(this.msgObj.timer);
this.msgObj.timer=null;
}
this.innerHTML="";
};
return ai2;
}
this.GetLocation=function (ele){
if ((ele.window&&ele.window===ele)||ele.nodeType===9)return {x:0,y:0};
var aj7=0;
var aj8=0;
var aj9=null;
var ak0=null;
var ak1=null;
for (var i4=ele;i4;aj9=i4,ak0=ak1,i4=i4.offsetParent){
var ah5=i4.tagName;
ak1=this.GetCurrentStyle2(i4);
if ((i4.offsetLeft||i4.offsetTop)&&
!((ah5==="BODY")&&
(!ak0||ak0.position!=="absolute"))){
aj7+=i4.offsetLeft;
aj8+=i4.offsetTop;
}
if (aj9!==null&&ak1){
if ((ah5!=="TABLE")&&(ah5!=="TD")&&(ah5!=="HTML")){
aj7+=parseInt(ak1.borderLeftWidth)||0;
aj8+=parseInt(ak1.borderTopWidth)||0;
}
if (ah5==="TABLE"&&
(ak1.position==="relative"||ak1.position==="absolute")){
aj7+=parseInt(ak1.marginLeft)||0;
aj8+=parseInt(ak1.marginTop)||0;
}
}
}
ak1=this.GetCurrentStyle2(ele);
var ak2=ak1?ak1.position:null;
if (!ak2||(ak2!=="absolute")){
for (var i4=ele.parentNode;i4;i4=i4.parentNode){
ah5=i4.tagName;
if ((ah5!=="BODY")&&(ah5!=="HTML")&&(i4.scrollLeft||i4.scrollTop)){
aj7-=(i4.scrollLeft||0);
aj8-=(i4.scrollTop||0);
ak1=this.GetCurrentStyle2(i4);
if (ak1){
aj7+=parseInt(ak1.borderLeftWidth)||0;
aj8+=parseInt(ak1.borderTopWidth)||0;
}
}
}
}
return {x:aj7,y:aj8};
}
var ak3=["borderTopWidth","borderRightWidth","borderBottomWidth","borderLeftWidth"];
var ak4=["borderTopStyle","borderRightStyle","borderBottomStyle","borderLeftStyle"];
var ak5;
this.GetBorderWidth=function (ele,side){
if (!this.GetBorderVisible(ele,side))return 0;
var m5=this.GetCurrentStyle(ele,ak3[side]);
return this.ParseBorderWidth(m5);
}
this.GetBorderVisible=function (ele,side){
return this.GetCurrentStyle(ele,ak4[side])!="none";
}
this.GetWindow=function (ele){
var af2=ele.ownerDocument||ele.document||ele;
return af2.defaultView||af2.parentWindow;
}
this.GetCurrentStyle2=function (ele){
if (ele.nodeType===3)return null;
var i9=this.GetWindow(ele);
if (ele.documentElement)ele=ele.documentElement;
var ak6=(i9&&(ele!==i9))?i9.getComputedStyle(ele,null):ele.style;
return ak6;
}
this.GetCurrentStyle=function (ele,attribute,defaultValue){
var ak7=null;
if (ele){
if (ele.currentStyle){
ak7=ele.currentStyle[attribute];
}
else if (document.defaultView&&document.defaultView.getComputedStyle){
var ak8=document.defaultView.getComputedStyle(ele,null);
if (ak8){
ak7=ak8[attribute];
}
}
if (!ak7&&ele.style.getPropertyValue){
ak7=ele.style.getPropertyValue(attribute);
}
else if (!ak7&&ele.style.getAttribute){
ak7=ele.style.getAttribute(attribute);
}
}
if (!ak7||ak7==""||typeof(ak7)==='undefined'){
if (typeof(defaultValue)!='undefined'){
ak7=defaultValue;
}
else {
ak7=null;
}
}
return ak7;
}
this.ParseBorderWidth=function (m5){
if (!ak5){
var ak9={};
var al0=document.createElement('div');
al0.style.visibility='hidden';
al0.style.position='absolute';
al0.style.fontSize='1px';
document.body.appendChild(al0)
var al1=document.createElement('div');
al1.style.height='0px';
al1.style.overflow='hidden';
al0.appendChild(al1);
var al2=al0.offsetHeight;
al1.style.borderTop='solid black';
al1.style.borderTopWidth='thin';
ak9['thin']=al0.offsetHeight-al2;
al1.style.borderTopWidth='medium';
ak9['medium']=al0.offsetHeight-al2;
al1.style.borderTopWidth='thick';
ak9['thick']=al0.offsetHeight-al2;
al0.removeChild(al1);
document.body.removeChild(al0);
ak5=ak9;
}
if (m5){
switch (m5){
case 'thin':
case 'medium':
case 'thick':
return ak5[m5];
case 'inherit':
return 0;
}
var al3=this.ParseUnit(m5);
if (al3.type!='px')
throw new Error();
return al3.size;
}
return 0;
}
this.ParseUnit=function (m5){
if (!m5)
throw new Error();
m5=this.Trim(m5).toLowerCase();
var aa0=m5.length;
var r3=-1;
for (var e9=0;e9<aa0;e9++){
var y1=m5.substr(e9,1);
if ((y1<'0'||y1>'9')&&y1!='-'&&y1!='.'&&y1!=',')
break ;
r3=e9;
}
if (r3==-1)
throw new Error();
var ah6;
var al4;
if (r3<(aa0-1))
ah6=this.Trim(m5.substring(r3+1));
else 
ah6='px';
al4=parseFloat(m5.substr(0,r3+1));
if (ah6=='px'){
al4=Math.floor(al4);
}
return {size:al4,type:ah6};
}
this.GetViewPortByRowCol=function (e4,i5,m7){
var al5=null;
var f1=null;
var al6=null;
var m5=this.GetViewport(e4);
var h2=this.GetCellByRowCol(e4,i5,m7);
if (m5!=null&&this.IsChild(h2,m5))
return m5;
else if (al6!=null&&this.IsChild(h2,al6))
return al6;
else if (f1!=null&&this.IsChild(h2,f1))
return f1;
else if (al5!=null&&this.IsChild(h2,al5))
return al5;
return ;
}
this.GetMsgPos=function (e4,i5,m7){
if (i5<0||m7<0){
return {left:0,top:0};
}
else {
var al5=null;
var f1=null;
var al6=null;
var m5=this.GetViewport(e4);
var al7=this.GetGroupBar(e4);
var k8=document.getElementById(e4.id+"_titleBar");
var h2=this.GetCellByRowCol(e4,i5,m7);
var f7=h2.offsetTop+h2.offsetHeight;
var aa0=h2.offsetLeft;
if ((al5!=null||f1!=null)&&(this.IsChild(h2,al6)||this.IsChild(h2,m5))){
if (al5!=null)
f7+=al5.offsetHeight;
else 
f7+=f1.offsetHeight;
}
if ((al5!=null||al6!=null)&&(this.IsChild(h2,f1)||this.IsChild(h2,m5))){
if (al5!=null)
aa0+=al5.offsetWidth;
else 
aa0+=al6.offsetWidth;
}
if (m5!=null&&(al5||f1||al6)){
if (k8)f7+=k8.offsetHeight;
if (al7)f7+=al7.offsetHeight;
if (this.GetColHeader(e4))f7+=this.GetColHeader(e4).offsetHeight;
if (this.GetRowHeader(e4))aa0+=this.GetRowHeader(e4).offsetWidth;
}
if (m5!=null&&this.IsChild(h2,m5)){
if (f1)
f7-=m5.parentNode.scrollTop;
if (al6)
aa0-=m5.parentNode.scrollLeft;
}
if (al6!=null&&this.IsChild(h2,al6)){
f7-=al6.parentNode.scrollTop;
}
if (f1!=null&&this.IsChild(h2,f1)){
aa0-=f1.parentNode.scrollLeft;
}
var j7=h2.clientHeight;
var i9=h2.clientWidth;
return {left:aa0,top:f7,a4:j7,a5:i9};
}
}
this.SyncMsgs=function (e4){
if (!e4.msgList)return ;
for (e9 in e4.msgList){
if (e4.msgList[e9].constructor==Array){
for (h9 in e4.msgList[e9]){
if (e4.msgList[e9][h9]&&e4.msgList[e9][h9].msgBox&&e4.msgList[e9][h9].msgBox.IsVisible){
e4.msgList[e9][h9].msgBox.Show(e4,this);
}
}
}
}
}
this.GetCellInfo=function (e4,g9,h1,v4){
var f3=this.GetData(e4);
if (f3==null)return null;
var f4=f3.getElementsByTagName("root")[0];
if (f4==null)return null;
var n1=f4.getElementsByTagName("state")[0];
if (n1==null)return null;
var al8=n1.getElementsByTagName("cellinfo")[0];
if (al8==null)return null;
var f7=al8.firstChild;
while (f7!=null){
if ((f7.getAttribute("r")==""+g9)&&(f7.getAttribute("c")==""+h1)&&(f7.getAttribute("pos")==""+v4))return f7;
f7=f7.nextSibling;
}
return null;
}
this.AddCellInfo=function (e4,g9,h1,v4){
var m9=this.GetCellInfo(e4,g9,h1,parseInt(v4));
if (m9!=null)return m9;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
if (f4==null)return null;
var n1=f4.getElementsByTagName("state")[0];
if (n1==null)return null;
var al8=n1.getElementsByTagName("cellinfo")[0];
if (al8==null)return null;
if (document.all!=null){
m9=f3.createNode("element","c","");
}else {
m9=document.createElement("c");
m9.style.display="none";
}
m9.setAttribute("r",g9);
m9.setAttribute("c",h1);
m9.setAttribute("pos",v4);
al8.appendChild(m9);
return m9;
}
this.setCellAttribute=function (e4,h2,attname,u2,noEvent,recalc){
if (h2==null)return ;
var g9=this.GetRowKeyFromCell(e4,h2);
var h1=this.GetColKeyFromCell(e4,h2);
if (typeof(g9)=="undefined")return ;
var v4=-1;
if (this.IsChild(h2,this.GetCorner(e4)))
v4=0;
else if (this.IsChild(h2,this.GetRowHeader(e4)))
v4=1;
else if (this.IsChild(h2,this.GetColHeader(e4)))
v4=2;
else if (this.IsChild(h2,this.GetViewport(e4)))
v4=3;
var q9=this.AddCellInfo(e4,g9,h1,v4);
q9.setAttribute(attname,u2);
if (!noEvent){
var g0=this.CreateEvent("DataChanged");
g0.cell=h2;
g0.cellValue=u2;
g0.row=g9;
g0.col=h1;
this.FireEvent(e4,g0);
}
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.updateCellLocked=function (h2,locked){
if (h2==null)return ;
var f7=h2.getAttribute("FpCellType")=="readonly";
if (f7==locked)return ;
var h1=h2.firstChild;
while (h1!=null){
if (typeof(h1.disabled)!="undefined")h1.disabled=locked;
h1=h1.nextSibling;
}
}
this.Cells=function (e4,g9,h1)
{
var al9=this.GetCellByRowCol(e4,g9,h1);
if (al9){
al9.GetValue=function (){
return the_fpSpread.GetValue(e4,g9,h1);
}
al9.SetValue=function (value){
if (typeof(value)=="undefined")return ;
if (this.parentNode.getAttribute("previewRow")!=null)return ;
the_fpSpread.SetValue(e4,g9,h1,value);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetBackColor=function (){
if (this.getAttribute("bgColorBak")!=null)
return this.getAttribute("bgColorBak");
return document.defaultView.getComputedStyle(this,"").getPropertyValue("background-color");
}
al9.SetBackColor=function (value){
if (typeof(value)=="undefined")return ;
this.bgColor=value;
this.setAttribute("bgColorBak",value);
this.style.backgroundColor=value;
the_fpSpread.setCellAttribute(e4,this,"bc",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetForeColor=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("color");
}
al9.SetForeColor=function (value){
if (typeof(value)=="undefined")return ;
this.style.color=value;
the_fpSpread.setCellAttribute(e4,this,"fc",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetTabStop=function (){
return this.getAttribute("TabStop")!="false";
}
al9.SetTabStop=function (value){
var am0=new String(value);
if (am0.toLocaleLowerCase()=="false"){
this.setAttribute("TabStop","false");
the_fpSpread.setCellAttribute(e4,this,"ts","false");
the_fpSpread.SaveClientEditedDataRealTime();
}else {
this.removeAttribute("TabStop");
}
}
al9.GetCellType=function (){
var am1=the_fpSpread.GetCellType2(this);
if (am1=="text"||am1=="readonly")
{
am1=this.getAttribute("CellType2");
}
if (am1==null)
am1="GeneralCellType";
return am1;
}
al9.GetHAlign=function (){
var am2=document.defaultView.getComputedStyle(this,"").getPropertyValue("text-Align");
if (am2==""||am2=="undefined"||am2==null)
am2=this.style.textAlign;
if (am2==""||am2=="undefined"||am2==null)
am2=this.getAttribute("align");
if (am2!=null&&am2.indexOf("-webkit")!=-1)am2=am2.replace("-webkit-","");
return am2;
}
al9.SetHAlign=function (value){
if (typeof(value)=="undefined")return ;
this.style.textAlign=typeof(value)=="string"?value:value.Name;
the_fpSpread.setCellAttribute(e4,this,"ha",typeof(value)=="string"?value:value.Name);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetVAlign=function (){
var am3=document.defaultView.getComputedStyle(this,"").getPropertyValue("vertical-Align");
if (am3==""||am3=="undefined"||am3==null)
am3=this.style.verticalAlign;
if (am3==""||am3=="undefined"||am3==null)
am3=this.getAttribute("valign");
return am3;
}
al9.SetVAlign=function (value){
if (typeof(value)=="undefined")return ;
this.style.verticalAlign=typeof(value)=="string"?value:value.Name;
the_fpSpread.setCellAttribute(e4,this,"va",typeof(value)=="string"?value:value.Name);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetLocked=function (){
if (al9.GetCellType()=="ButtonCellType"||al9.GetCellType()=="TagCloudCellType"||al9.GetCellType()=="HyperLinkCellType")
return al9.getAttribute("Locked")=="1";
return the_fpSpread.GetCellType(this)=="readonly";
}
al9.GetFont_Name=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-family");
}
al9.SetFont_Name=function (value){
if (typeof(value)=="undefined")return ;
this.style.fontFamily=value;
the_fpSpread.setCellAttribute(e4,this,"fn",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetFont_Size=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-size");
}
al9.SetFont_Size=function (value){
if (typeof(value)=="undefined")return ;
if (typeof(value)=="number")value+="px";
this.style.fontSize=value;
the_fpSpread.setCellAttribute(e4,this,"fs",value);
the_fpSpread.SizeSpread(e4);
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetFont_Bold=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-weight")=="bold"?true:false;
}
al9.SetFont_Bold=function (value){
if (typeof(value)=="undefined")return ;
this.style.fontWeight=value==true?"bold":"normal";
the_fpSpread.setCellAttribute(e4,this,"fb",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetFont_Italic=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("font-style")=="italic"?true:false;
}
al9.SetFont_Italic=function (value){
if (typeof(value)=="undefined")return ;
this.style.fontStyle=value==true?"italic":"normal";
the_fpSpread.setCellAttribute(e4,this,"fi",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetFont_Overline=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0?true:false;
}
al9.SetFont_Overline=function (value){
if (value){
var am4=new String("overline");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
am4+=" line-through"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
am4+=" underline"
this.style.textDecoration=am4;
}
else {
var am4=new String("");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
am4+=" line-through"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
am4+=" underline"
if (am4=="")am4="none";
this.style.textDecoration=am4;
}
the_fpSpread.setCellAttribute(e4,this,"fo",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetFont_Strikeout=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0?true:false;
}
al9.SetFont_Strikeout=function (value){
if (value){
var am4=new String("line-through");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
am4+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
am4+=" underline"
this.style.textDecoration=am4;
}
else {
var am4=new String("");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
am4+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0)
am4+=" underline"
if (am4=="")am4="none";
this.style.textDecoration=am4;
}
the_fpSpread.setCellAttribute(e4,this,"fk",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
al9.GetFont_Underline=function (){
return document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("underline")>=0?true:false;
}
al9.SetFont_Underline=function (value){
if (value){
var am4=new String("underline");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
am4+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
am4+=" line-through"
this.style.textDecoration=am4;
}
else {
var am4=new String("");
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("overline")>=0)
am4+=" overline"
if (document.defaultView.getComputedStyle(this,"").getPropertyValue("text-decoration").indexOf("line-through")>=0)
am4+=" line-through"
if (am4=="")am4="none";
this.style.textDecoration=am4;
}
the_fpSpread.setCellAttribute(e4,this,"fu",new String(value).toLocaleLowerCase());
the_fpSpread.SaveClientEditedDataRealTime();
}
return al9;
}
return null;
}
this.getDomRow=function (e4,g9){
var n0=this.GetRowCount(e4);
if (n0==0)return null;
var h2=this.GetCellByRowCol(e4,g9,0);
if (h2){
var e8=h2.parentNode.rowIndex;
if (e8>=0){
var i5=h2.parentNode.parentNode.rows[e8];
if (this.GetSizable(e4,i5))
return i5;
}
return null;
}
}
this.setRowInfo_RowAttribute=function (e4,g9,attname,u2,recalc){
g9=parseInt(g9);
if (g9<0)return ;
var am5=this.AddRowInfo(e4,g9);
am5.setAttribute(attname,u2);
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.Rows=function (e4,g9)
{
var am6=this.getDomRow(e4,g9);
if (am6){
am6.GetHeight=function (){
return the_fpSpread.GetRowHeightInternal(e4,g9);
}
am6.SetHeight=function (ac3){
if (typeof(ac3)=="undefined")return ;
if (ac3<1)
ac3=1;
g9=the_fpSpread.GetDisplayIndex(e4,g9);
var b5=null;
if (the_fpSpread.GetRowHeader(e4)!=null)b5=the_fpSpread.GetRowHeader(e4).rows[g9];
if (b5!=null)b5.cells[0].style.posHeight=ac3;
var i3=the_fpSpread.GetViewport(e4);
if (b5==null)
b5=i3.rows[g9];
if (b5!=null)b5.cells[0].style.posHeight=ac3;
var p7=the_fpSpread.AddRowInfo(e4,b5.getAttribute("FpKey"));
if (p7!=null){
if (typeof(b5.cells[0].style.posHeight)=="undefined")
the_fpSpread.SetRowHeight(e4,p7,ac3);
else 
the_fpSpread.SetRowHeight(e4,p7,b5.cells[0].style.posHeight);
}
var i4=the_fpSpread.GetParentSpread(e4);
if (i4!=null)i4.UpdateRowHeight(e4);
the_fpSpread.SynRowHeight(e4,the_fpSpread.GetRowHeader(e4),i3,g9,true,false)
var e7=the_fpSpread.GetTopSpread(e4);
the_fpSpread.SizeAll(e7);
the_fpSpread.Refresh(e7);
the_fpSpread.SaveClientEditedDataRealTime();
}
return am6;
}
return null;
}
this.setColInfo_ColumnAttribute=function (e4,h1,attname,u2,recalc){
h1=parseInt(h1);
if (h1<0)return ;
var am7=this.AddColInfo(e4,h1);
am7.setAttribute(attname,u2);
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.Columns=function (e4,h1)
{
var am8={a2:this.GetColByKey(e4,parseInt(h1))};
if (am8){
am8.GetWidth=function (){
return the_fpSpread.GetColWidthFromCol(e4,h1);
}
am8.SetWidth=function (value){
if (typeof(value)=="undefined")return ;
the_fpSpread.SetColWidth(e4,h1,value);
the_fpSpread.SaveClientEditedDataRealTime();
}
return am8;
}
return null;
}
this.GetTitleBar=function (e4){
try {
if (document.getElementById(e4.id+"_title")==null)return null;
var am9=document.getElementById(e4.id+"_titleBar");
if (am9!=null)am9=document.getElementById(e4.id+"_title");
return am9;
}
catch (ex){
return null;
}
}
this.CheckTitleInfo=function (e4){
var f3=this.GetData(e4);
if (f3==null)return null;
var f4=f3.getElementsByTagName("root")[0];
if (f4==null)return null;
var an0=f4.getElementsByTagName("titleinfo")[0];
if (an0==null)return null;
return an0;
}
this.AddTitleInfo=function (e4){
var m9=this.CheckTitleInfo(e4);
if (m9!=null)return m9;
var f3=this.GetData(e4);
var f4=f3.getElementsByTagName("root")[0];
if (f4==null)return null;
if (document.all!=null){
m9=f3.createNode("element","titleinfo","");
}else {
m9=document.createElement("titleinfo");
m9.style.display="none";
}
f4.appendChild(m9);
return m9;
}
this.setTitleInfo_Attribute=function (e4,attname,u2,recalc){
var an1=this.AddTitleInfo(e4);
an1.setAttribute(attname,u2);
var f6=this.GetCmdBtn(e4,"Update");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
f6=this.GetCmdBtn(e4,"Cancel");
if (f6!=null&&f6.getAttribute("disabled")!=null)
this.UpdateCmdBtnState(f6,false);
e4.e2=true;
if (recalc){
this.UpdateValues(e4);
}
}
this.GetTitleInfo=function (e4)
{
var an2=this.GetTitleBar(e4);
if (an2){
an2.GetHeight=function (){
return this.style.height;
}
an2.SetHeight=function (value){
this.style.height=parseInt(value)+"px";
the_fpSpread.setTitleInfo_Attribute(e4,"ht",value);
var e7=the_fpSpread.GetTopSpread(e4);
the_fpSpread.SizeAll(e7);
the_fpSpread.Refresh(e7);
the_fpSpread.SaveClientEditedDataRealTime();
}
an2.GetVisible=function (){
return (document.defaultView.getComputedStyle(this,"").getPropertyValue("display")=="none")?false:true;
return document.defaultView.getComputedStyle(this,"").getPropertyValue("visibility");
}
an2.SetVisible=function (value){
this.style.display=value?"":"none";
this.style.visibility=value?"visible":"hidden";
the_fpSpread.setTitleInfo_Attribute(e4,"vs",new String(value).toLocaleLowerCase());
var e7=the_fpSpread.GetTopSpread(e4);
the_fpSpread.SizeAll(e7);
the_fpSpread.Refresh(e7);
the_fpSpread.SaveClientEditedDataRealTime();
}
an2.GetValue=function (){
return this.textContent;
}
an2.SetValue=function (value){
this.textContent=""+value;
the_fpSpread.setTitleInfo_Attribute(e4,"tx",value);
the_fpSpread.SaveClientEditedDataRealTime();
}
return an2;
}
return null;
}
this.SaveClientEditedDataRealTime=function (){
var an3=this.GetPageActiveSpread();
if (an3!=null){
this.SaveData(an3);
an3.e2=false;
}
an3=this.GetPageActiveSheetView();
if (an3!=null){
this.SaveData(an3);
an3.e2=false;
}
}
var an4="";
this.ShowScrollingContent=function (e4,hs){
var r0="";
var o4=this.GetTopSpread(e4);
var an5=o4.getAttribute("scrollContentColumns");
var an6=o4.getAttribute("scrollContentMaxHeight");
var an7=o4.getAttribute("scrollContentTime");
var i3=this.GetViewport(o4);
var an8=this.GetColGroup(i3);
var m5=this.GetParent(i3);
var an9=0;
if (hs){
var ao0=m5.scrollLeft;
var c6=this.GetColHeader(o4);
var r7=0;
for (;r7<an8.childNodes.length;r7++){
var h1=an8.childNodes[r7];
an9+=parseInt(h1.width);
if (an9>ao0)break ;
}
var ao1=null;
if (ao1)r7+=this.GetColGroup(ao1).childNodes.length;
if (c6){
var r6=c6.rows.length-1;
if (e4.getAttribute("LayoutMode")==null)
r6=parseInt(c6.getAttribute("ColTextIndex"))?c6.getAttribute("ColTextIndex"):c6.rows.length-1;
var ao2=this.GetHeaderCellFromRowCol(o4,r6,r7,true);
if (ao2){
if (ao2.getAttribute("FpCellType")=="ExtenderCellType"&&ao2.getElementsByTagName("DIV").length>0){
var w3=this.GetEditor(ao2);
var w4=this.GetFunction("ExtenderCellType_getEditorValue");
if (w3!==null&&w4!==null){
r0="&nbsp;Column:&nbsp;"+w4(w3)+"&nbsp;";
}
}
else 
r0="&nbsp;Column:&nbsp;"+ao2.innerHTML+"&nbsp;";
}
}
if (r0.length<=0)r0="&nbsp;Column:&nbsp;"+(r7+1)+"&nbsp;";
}
else {
var n1=m5.scrollTop;
var c5=this.GetRowHeader(o4);
var r6=0;
var ao3=0;
var ao4=2;
for (var x4=0;x4<i3.rows.length;x4++){
var g9=i3.rows[x4];
an9+=g9.offsetHeight;
if (an9>n1){
if (g9.getAttribute("fpkey")==null&&g9.getAttribute("previewrow")!="true")
r6--;
else 
ao3=g9.offsetHeight;
break ;
}
if (g9.getAttribute("fpkey")!=null||g9.getAttribute("previewrow")=="true"){
r6++;
ao3=g9.offsetHeight;
}
}
var ao1=null;
if (ao1)r6+=ao1.rows.length;
if (e4.getAttribute("LayoutMode")==null&&an5!=null&&an5.length>0){
ao3=ao3>an6?an6:ao3;
var ao5=an5.split(",");
var ao6=false;
for (var e9=0;e9<ao5.length;e9++){
var h1=parseInt(ao5[e9]);
if (h1==null||h1>=this.GetColCount(e4))continue ;
var h2=o4.GetCellByRowCol(r6,h1);
if (!h2||h2.getAttribute("col")!=null&&h2.getAttribute("col")!=h1)continue ;
var ao7=(h2.getAttribute("group")==1);
var ab3=(h2.parentNode.getAttribute("previewrow")=="true");
var g0=(h2.getAttribute("RowEditTemplate")!=null);
var j8=this.IsXHTML(e4);
if (!j8&&an4==""){
this.GetScrollingContentStyle(e4);
if (ai5!=null){
if (ai5.fontFamily!=null&&ai5.fontFamily!="")an4+="fontFamily:"+ai5.fontFamily+";";
if (ai5.fontSize!=null&&ai5.fontSize!="")an4+="fontSize:"+ai5.fontSize+";";
if (ai5.fontStyle!=null&&ai5.fontStyle!="")an4+="fontStyle:"+ai5.fontStyle+";";
if (ai5.fontVariant!=null&&ai5.fontVariant!="")an4+="fontVariant:"+ai5.fontVariant+";";
if (ai5.fontWeight!=null&&ai5.fontWeight!="")an4+="fontWeight:"+ai5.fontWeight+";";
if (ai5.backgroundColor!=null&&ai5.backgroundColor!="")an4+="backgroundColor:"+ai5.backgroundColor+";";
if (ai5.color!=null&&ai5.color!="")an4+="color:"+ai5.color;
}
}
if (!ao6){
r0+="<div style='overflow:hidden;height:"+ao3+"px;ScrollingContentWidth'><table cellPadding='0' cellSpacing='0' style='height:"+ao3+"px;"+(ao7?"":"table-layout:auto;")+an4+"'><tr>";
}
r0+="<td style='width:"+(ao7?0:h2.offsetWidth)+"px;'>";
ao4+=h2.offsetWidth;
if (ao7)
r0+="&nbsp;<i>GroupBar:</i>&nbsp;"+h2.textContent+"&nbsp;";
else if (ab3)
r0+="&nbsp;<i>PreviewRow:</i>&nbsp;"+h2.textContent+"&nbsp;";
else if (g0){
var ao8=this.parseCell(e4,h2);
r0+="&nbsp;<i>RowEditTemplate:</i>&nbsp;"+ao8+"&nbsp;";
}
else {
if (h2.getAttribute("fpcelltype"))this.UpdateCellTypeDOM(h2);
if (h2.getAttribute("fpcelltype")=="MultiColumnComboBoxCellType"&&h2.childNodes[0]&&h2.childNodes[0].childNodes.length>0&&h2.childNodes[0].getAttribute("MccbId"))
r0+=o4.GetValue(r6,h1);
else if (h2.getAttribute("fpcelltype")=="RadioButtonListCellType"||h2.getAttribute("fpcelltype")=="ExtenderCellType"||h2.getAttribute("fpeditorid")!=null){
var ao9=this.parseCell(e4,h2);
r0+=ao9;
}
else 
r0+=h2.innerHTML;
}
r0+="</td>";
ao6=true;
if (ao7||ab3||g0)break ;
}
if (ao6){
r0=r0.replace("ScrollingContentWidth"," width:"+ao4+"px;");
r0+="</tr></table></div>";
}
}
if (r0.length<=0&&c5){
var r7=this.GetColGroup(c5).childNodes.length-1;
if (e4.getAttribute("LayoutMode")==null)
r7=c5.getAttribute("RowTextIndex")?parseInt(c5.getAttribute("RowTextIndex"))+1:this.GetColGroup(c5).childNodes.length-1;
var ao2=this.GetHeaderCellFromRowCol(e4,r6,r7,false);
if (ao2)r0="&nbsp;Row:&nbsp;"+ao2.textContent+"&nbsp;";
}
if (r0.length<=0)r0="&nbsp;Row:&nbsp;"+(r6+1)+"&nbsp;";
}
this.ShowMessageInner(o4,r0,(hs?-1:-2),(hs?-2:-1),an7);
}
this.parseCell=function (e4,h2){
var r0=h2.innerHTML;
var o4=this.GetTopSpread(e4);
var ap0=o4.id;
if (r0.length>0){
r0=r0.replace(new RegExp("=\""+ap0,"g"),"=\""+ap0+"src");
r0=r0.replace(new RegExp("name="+ap0,"g"),"name="+ap0+"src");
}
return r0;
}
this.UpdateCellTypeDOM=function (h2){
for (var e9=0;e9<h2.childNodes.length;e9++){
if (h2.childNodes[e9].tagName&&(h2.childNodes[e9].tagName=="INPUT"||h2.childNodes[e9].tagName=="SELECT"))
this.UpdateDOM(h2.childNodes[e9]);
if (h2.childNodes[e9].childNodes&&h2.childNodes[e9].childNodes.length>0)
this.UpdateCellTypeDOM(h2.childNodes[e9]);
}
}
this.UpdateDOM=function (inputField){
if (typeof(inputField)=="string"){
inputField=document.getElementById(inputField);
}
if (inputField.type=="select-one"){
for (var e9=0;e9<inputField.options.length;e9++){
if (e9==inputField.selectedIndex){
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
if (ai5!=null)return ;
var e8=document.styleSheets.length;
for (var e9=0;e9<e8;e9++){
var ap1=document.styleSheets[e9];
for (var h9=0;h9<ap1.cssRules.length;h9++){
var ap2=ap1.cssRules[h9];
if (ap2.selectorText=="."+e4.id+"scrollContentStyle"||ap2.selectorText=="."+e4.id.toLowerCase()+"scrollcontentstyle"){
ai5=ap2.style;
break ;
}
}
if (ai5!=null)break ;
}
}
}
function CheckBoxCellType_setFocus(h2){
var i2=h2.getElementsByTagName("INPUT");
if (i2!=null&&i2.length>0&&i2[0].type=="checkbox"){
i2[0].focus();
}
}
function CheckBoxCellType_getCheckBoxEditor(h2){
var i2=h2.getElementsByTagName("INPUT");
if (i2!=null&&i2.length>0&&i2[0].type=="checkbox"){
return i2[0];
}
return null;
}
function CheckBoxCellType_isValid(h2,u2){
if (u2==null)return "";
u2=the_fpSpread.Trim(u2);
if (u2=="")return "";
if (u2.toLowerCase()=="true"||u2.toLowerCase()=="false")
return "";
else 
return "invalid value";
}
function CheckBoxCellType_getValue(u6,e4){
return CheckBoxCellType_getEditorValue(u6,e4);
}
function CheckBoxCellType_getEditorValue(u6,e4){
var h2=the_fpSpread.GetCell(u6);
var i2=CheckBoxCellType_getCheckBoxEditor(h2);
if (i2!=null&&i2.checked){
return "true";
}
return "false";
}
function CheckBoxCellType_setValue(u6,u2){
var h2=the_fpSpread.GetCell(u6);
var i2=CheckBoxCellType_getCheckBoxEditor(h2);
if (i2!=null){
i2.checked=(u2!=null&&u2.toLowerCase()=="true");
return ;
}
}
function IntegerCellType_getValue(u6){
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
if (f7.innerHTML=="&nbsp;")return "";
var s6=f7.innerHTML;
u6=the_fpSpread.GetCell(u6);
if (u6.getAttribute("FpRef")!=null)u6=document.getElementById(u6.getAttribute("FpRef"));
var ap3=u6.getAttribute("groupchar");
if (ap3==null)ap3=",";
var t3=s6.length;
while (true){
s6=s6.replace(ap3,"");
if (s6.length==t3)break ;
t3=s6.length;
}
if (s6.charAt(0)=='('&&s6.charAt(s6.length-1)==')'){
var ap4=u6.getAttribute("negsign");
if (ap4==null)ap4="-";
s6=ap4+s6.substring(1,s6.length-1);
}
s6=the_fpSpread.ReplaceAll(s6,"&nbsp;"," ");
return s6;
}
function IntegerCellType_isValid(h2,u2){
if (u2==null||u2.length==0)return "";
u2=u2.replace(" ","");
if (u2.length==0)return "";
var an9=h2;
var ap5=h2.getAttribute("FpRef");
if (ap5!=null)an9=document.getElementById(ap5);
var ap4=an9.getAttribute("negsign");
var v4=an9.getAttribute("possign");
if (ap4!=null)u2=u2.replace(ap4,"-");
if (v4!=null)u2=u2.replace(v4,"+");
if (u2.charAt(u2.length-1)=="-")u2="-"+u2.substring(0,u2.length-1);
var t5=new RegExp("^\\s*[-\\+]?\\d+\\s*$");
var o0=(u2.match(t5)!=null);
if (o0)o0=!isNaN(u2);
if (o0){
var t9=an9.getAttribute("MinimumValue");
var i7=an9.getAttribute("MaximumValue");
var t8=parseInt(u2);
if (t9!=null){
t9=parseInt(t9);
o0=(!isNaN(t9)&&t8>=t9);
}
if (o0&&i7!=null){
i7=parseInt(i7);
o0=(!isNaN(i7)&&t8<=i7);
}
}
if (!o0){
if (an9.getAttribute("error")!=null)
return an9.getAttribute("error");
else 
return "Integer";
}
return "";
}
function DoubleCellType_isValid(h2,u2){
if (u2==null||u2.length==0)return "";
var an9=h2;
if (h2.getAttribute("FpRef")!=null)an9=document.getElementById(h2.getAttribute("FpRef"));
var ap6=an9.getAttribute("decimalchar");
if (ap6==null)ap6=".";
var ap3=an9.getAttribute("groupchar");
if (ap3==null)ap3=",";
u2=the_fpSpread.Trim(u2);
var o0=true;
o0=(u2.length==0||u2.charAt(0)!=ap3);
if (o0){
var t3=u2.length;
while (true){
u2=u2.replace(ap3,"");
if (u2.length==t3)break ;
t3=u2.length;
}
}
var o0=true;
if (u2.length==0){
o0=false;
}else if (o0){
var ap4=an9.getAttribute("negsign");
var v4=an9.getAttribute("possign");
var t9=an9.getAttribute("MinimumValue");
var i7=an9.getAttribute("MaximumValue");
o0=the_fpSpread.IsDouble(u2,ap6,ap4,v4,t9,i7);
}
if (!o0){
if (an9.getAttribute("error")!=null)
return an9.getAttribute("error");
else 
return "Double";
}
return "";
}
function DoubleCellType_getValue(u6){
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
if (f7.innerHTML=="&nbsp;")return "";
var s6=f7.innerHTML;
u6=the_fpSpread.GetCell(u6);
if (u6.getAttribute("FpRef")!=null)u6=document.getElementById(u6.getAttribute("FpRef"));
var ap3=u6.getAttribute("groupchar");
if (ap3==null)ap3=",";
var t3=s6.length;
while (true){
s6=s6.replace(ap3,"");
if (s6.length==t3)break ;
t3=s6.length;
}
if (s6.charAt(0)=='('&&s6.charAt(s6.length-1)==')'){
var ap4=u6.getAttribute("negsign");
if (ap4==null)ap4="-";
s6=ap4+s6.substring(1,s6.length-1);
}
s6=the_fpSpread.ReplaceAll(s6,"&nbsp;"," ");
return s6;
}
function CurrencyCellType_isValid(h2,u2){
if (u2!=null&&u2.length>0){
var an9=h2;
if (h2.getAttribute("FpRef")!=null)an9=document.getElementById(h2.getAttribute("FpRef"));
var t2=an9.getAttribute("currencychar");
if (t2==null)t2="$";
u2=u2.replace(t2,"");
var ap3=an9.getAttribute("groupchar");
if (ap3==null)ap3=",";
u2=the_fpSpread.Trim(u2);
var o0=true;
o0=(u2.length==0||u2.charAt(0)!=ap3);
if (o0){
var t3=u2.length;
while (true){
u2=u2.replace(ap3,"");
if (u2.length==t3)break ;
t3=u2.length;
}
}
if (u2.length==0){
o0=false;
}else if (o0){
var ap6=an9.getAttribute("decimalchar");
if (ap6==null)ap6=".";
var ap4=an9.getAttribute("negsign");
var v4=an9.getAttribute("possign");
var t9=an9.getAttribute("MinimumValue");
var i7=an9.getAttribute("MaximumValue");
o0=the_fpSpread.IsDouble(u2,ap6,ap4,v4,t9,i7);
}
if (!o0){
if (an9.getAttribute("error")!=null)
return an9.getAttribute("error");
else 
return "Currency ("+t2+"100"+ap6+"10) ";
}
}
return "";
}
function CurrencyCellType_getValue(u6){
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
if (f7.innerHTML=="&nbsp;")return "";
var s6=f7.innerHTML;
u6=the_fpSpread.GetCell(u6);
if (u6.getAttribute("FpRef")!=null)u6=document.getElementById(u6.getAttribute("FpRef"));
var t2=u6.getAttribute("currencychar");
if (t2!=null){
var ap7=document.createElement("SPAN");
ap7.innerHTML=t2;
t2=ap7.innerHTML;
}
if (t2==null)t2="$";
var ap3=u6.getAttribute("groupchar");
if (ap3==null)ap3=",";
s6=s6.replace(t2,"");
var t3=s6.length;
while (true){
s6=s6.replace(ap3,"");
if (s6.length==t3)break ;
t3=s6.length;
}
var ap4=u6.getAttribute("negsign");
if (ap4==null)ap4="-";
if (s6.charAt(0)=='('&&s6.charAt(s6.length-1)==')'){
s6=ap4+s6.substring(1,s6.length-1);
}
s6=the_fpSpread.ReplaceAll(s6,"&nbsp;"," ");
return s6;
}
function RegExpCellType_isValid(h2,u2){
if (u2==null||u2=="")
return "";
var an9=h2;
if (h2.getAttribute("FpRef")!=null)an9=document.getElementById(h2.getAttribute("FpRef"));
var ap8=new RegExp(an9.getAttribute("fpexpression"));
var t6=u2.match(ap8);
var m5=(t6!=null&&t6.length>0&&u2==t6[0]);
if (!m5){
if (an9.getAttribute("error")!=null)
return an9.getAttribute("error");
else 
return "invalid";
}
return "";
}
function PercentCellType_getValue(u6){
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
if (f7.innerHTML=="&nbsp;")return "";
f7=f7.innerHTML;
var h2=the_fpSpread.GetCell(u6);
var an9=h2;
if (h2.getAttribute("FpRef")!=null)an9=document.getElementById(h2.getAttribute("FpRef"));
var ap9=an9.getAttribute("percentchar");
if (ap9==null)ap9="%";
f7=f7.replace(ap9,"");
var ap3=an9.getAttribute("groupchar");
if (ap3==null)ap3=",";
var t3=f7.length;
while (true){
f7=f7.replace(ap3,"");
if (f7.length==t3)break ;
t3=f7.length;
}
var ap4=an9.getAttribute("negsign");
var v4=an9.getAttribute("possign");
f7=the_fpSpread.ReplaceAll(f7,"&nbsp;"," ");
var g2=f7;
if (ap4!=null)
f7=f7.replace(ap4,"-");
if (v4!=null)
f7=f7.replace(v4,"+");
var ap6=an9.getAttribute("decimalchar");
if (ap6!=null)
f7=f7.replace(ap6,".");
if (!isNaN(f7))
return g2;
else 
return u6.innerHTML;
}
function PercentCellType_setValue(u6,u2){
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
u6=f7;
if (u2!=null&&u2!=""){
var an9=the_fpSpread.GetCell(u6);
if (an9.getAttribute("FpRef")!=null)an9=document.getElementById(an9.getAttribute("FpRef"));
var ap9=an9.getAttribute("percentchar");
if (ap9==null)ap9="%";
u2=u2.replace(" ","");
u2=u2.replace(ap9,"");
u6.innerHTML=u2+ap9;
}else {
u6.innerHTML="";
}
}
function PercentCellType_isValid(h2,u2){
if (u2!=null){
var an9=the_fpSpread.GetCell(h2);
if (an9.getAttribute("FpRef")!=null)an9=document.getElementById(an9.getAttribute("FpRef"));
var ap9=an9.getAttribute("percentchar");
if (ap9==null)ap9="%";
u2=u2.replace(ap9,"");
var ap3=an9.getAttribute("groupchar");
if (ap3==null)ap3=",";
var t3=u2.length;
while (true){
u2=u2.replace(ap3,"");
if (u2.length==t3)break ;
t3=u2.length;
}
var aq0=u2;
var ap4=an9.getAttribute("negsign");
var v4=an9.getAttribute("possign");
if (ap4!=null)u2=u2.replace(ap4,"-");
if (v4!=null)u2=u2.replace(v4,"+");
var ap6=an9.getAttribute("decimalchar");
if (ap6!=null)
u2=u2.replace(ap6,".");
var o0=!isNaN(u2);
if (o0){
var aq1=an9.getAttribute("MinimumValue");
var aq2=an9.getAttribute("MaximumValue");
if (aq1!=null||aq2!=null){
var t9=parseFloat(aq1);
var i7=parseFloat(aq2);
o0=!isNaN(t9)&&!isNaN(i7);
if (o0){
if (ap6==null)ap6=".";
o0=the_fpSpread.IsDouble(aq0,ap6,ap4,v4,t9*100,i7*100);
}
}
}
if (!o0){
if (an9.getAttribute("error")!=null)
return an9.getAttribute("error");
else 
return "Percent:(ex,10"+ap9+")";
}
}
return "";
}
function ListBoxCellType_getValue(u6){
var f7=u6.getElementsByTagName("TABLE");
if (f7.length>0)
{
var g7=f7[0].rows;
for (var h9=0;h9<g7.length;h9++){
var h2=g7[h9].cells[0];
if (h2.selected=="true")
{
var aq3=h2;
while (aq3.firstChild!=null)aq3=aq3.firstChild;
var an9=aq3.nodeValue;
return an9;
}
}
}
return "";
}
function ListBoxCellType_setValue(u6,u2){
var f7=u6.getElementsByTagName("TABLE");
if (f7.length>0)
{
f7[0].style.width=(u6.clientWidth-6)+"px";
var g7=f7[0].rows;
for (var h9=0;h9<g7.length;h9++){
var h2=g7[h9].cells[0];
var aq3=h2;
while (aq3.firstChild!=null)aq3=aq3.firstChild;
var an9=aq3.nodeValue;
if (an9==u2){
h2.selected="true";
if (f7[0].parentNode.getAttribute("selectedBackColor")!="undefined")
h2.style.backgroundColor=f7[0].parentNode.getAttribute("selectedBackColor");
if (f7[0].parentNode.getAttribute("selectedForeColor")!="undefined")
h2.style.color=f7[0].parentNode.getAttribute("selectedForeColor");
}else {
h2.style.backgroundColor="";
h2.style.color="";
h2.selected="";
h2.bgColor="";
}
}
}
}
function TextCellType_getValue(u6){
var h2=the_fpSpread.GetCell(u6,true);
if (h2!=null&&h2.getAttribute("password")!=null){
if (h2!=null&&h2.getAttribute("value")!=null)
return h2.getAttribute("value");
else 
return "";
}else {
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
if (f7.innerHTML=="&nbsp;")return "";
var f7=the_fpSpread.ReplaceAll(f7.innerHTML,"&nbsp;"," ");
var f7=the_fpSpread.ReplaceAll(f7,"<br>","\n");
return f7;
}
}
function TextCellType_setValue(u6,u2){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
var f7=u6;
while (f7.firstChild!=null&&f7.firstChild.nodeName!="#text")f7=f7.firstChild;
u6=f7;
if (h2.getAttribute("password")!=null){
if (u2!=null&&u2!=""){
u2=u2.replace(" ","");
u6.innerHTML="";
for (var e9=0;e9<u2.length;e9++)
u6.innerHTML+="*";
h2.setAttribute("value",u2);
}else {
u6.innerHTML="";
h2.setAttribute("value","");
}
}else {
u2=the_fpSpread.ReplaceAll(u2,"\n","<br>");
u6.innerHTML=the_fpSpread.ReplaceAll(u2," ","&nbsp;");
}
}
function RadioButtonListCellType_getValue(u6){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
var aq4=h2.getElementsByTagName("INPUT");
for (var e9=0;e9<aq4.length;e9++){
if (aq4[e9].tagName=="INPUT"&&aq4[e9].checked){
return aq4[e9].value;
}
}
return "";
}
function RadioButtonListCellType_getEditorValue(u6){
return RadioButtonListCellType_getValue(u6);
}
function RadioButtonListCellType_setValue(u6,u2){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
if (u2!=null)u2=the_fpSpread.Trim(u2);
var aq4=h2.getElementsByTagName("INPUT");
for (var e9=0;e9<aq4.length;e9++){
if (aq4[e9].tagName=="INPUT"&&u2==the_fpSpread.Trim(aq4[e9].value)){
aq4[e9].checked=true;
break ;
}else {
if (aq4[e9].checked)aq4[e9].checked=false;
}
}
}
function RadioButtonListCellType_setFocus(u6){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
var i2=h2.getElementsByTagName("INPUT");
if (i2==null)return ;
for (var e9=0;e9<i2.length;e9++){
if (i2[e9].type=="radio"&&i2[e9].checked){
i2[e9].focus();
return ;
}
}
}
function MultiColumnComboBoxCellType_setValue(u6,u2,e4){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
var aq5=h2.getElementsByTagName("DIV");
if (aq5!=null&&aq5.length>0){
var aq6=h2.getElementsByTagName("input");
if (aq6!=null&&aq6.length>0)
aq6[0].value=u2;
return ;
}
if (u2!=null&&u2!="")
u6.textContent=u2;
else 
u6.innerHTML="&nbsp;";
}
function MultiColumnComboBoxCellType_getValue(u6,e4){
var s6=u6.textContent;
var i8=the_fpSpread.GetCell(u6,true);
var aq5=i8.getElementsByTagName("DIV");
if (aq5!=null&&aq5.length>0){
var aq6=i8.getElementsByTagName("input");
if (aq6!=null&&aq6.length>0)
return aq6[0].value;
return ;
}
if (!e4)return null;
var s7=the_fpSpread.GetCellEditorID(e4,i8);
var a8=null;
if (s7!=null&&typeof(s7)!="undefined"){
a8=the_fpSpread.GetCellEditor(e4,s7,true);
if (a8!=null){
var aq7=a8.getAttribute("MccbId");
if (aq7){
FarPoint.System.WebControl.MultiColumnComboBoxCellType.CheckInit(aq7);
var aq8=eval(aq7+"_Obj");
if (aq8!=null&&aq8.SetText!=null){
aq8.SetText(s6);
return s6;
}
}
}
return null;
}
return s6;
}
function MultiColumnComboBoxCellType_getEditorValue(u6,e4){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
var aq9=h2.getElementsByTagName("INPUT");
if (aq9!=null&&aq9.length>0){
var f7=aq9[0];
return f7.value;
}
return null;
}
function MultiColumnComboBoxCellType_setFocus(u6){
var h2=the_fpSpread.GetCell(u6);
var e4=the_fpSpread.GetSpread(h2);
if (h2==null)return ;
var ar0=h2.getElementsByTagName("DIV");
if (ar0!=null&&ar0.length>0){
var aq7=ar0[0].getAttribute("MccbId");
if (aq7){
var aq8=eval(aq7+"_Obj");
if (aq8!=null&&typeof(aq8.FocusForEdit)!="undefined"){
aq8.FocusForEdit();
}
}
}
}
function MultiColumnComboBoxCellType_setEditorValue(u6,editorValue,e4){
var h2=the_fpSpread.GetCell(u6,true);
if (h2==null)return ;
var s7=the_fpSpread.GetCellEditorID(e4,h2);
var a8=null;
if (s7!=null&&typeof(s7)!="undefined"){
a8=the_fpSpread.GetCellEditor(e4,s7,true);
if (a8!=null){
var aq7=a8.getAttribute("MccbId");
if (aq7){
FarPoint.System.WebControl.MultiColumnComboBoxCellType.CheckInit(aq7);
var aq8=eval(aq7+"_Obj");
if (aq8!=null&&aq8.SetText!=null){
aq8.SetText(editorValue);
}
}
}
}
}
function TagCloudCellType_getValue(u6,e4){
var s6=u6.textContent;
if (typeof(s6)!="undefined"&&s6!=null&&s6.length>0)
{
s6=the_fpSpread.ReplaceAll(s6,"<br>","");
s6=the_fpSpread.ReplaceAll(s6,"\n","");
s6=the_fpSpread.ReplaceAll(s6,"\t","");
var r1=new RegExp("\xA0","g");
s6=s6.replace(r1,String.fromCharCode(32));
s6=the_fpSpread.HTMLDecode(s6);
}
else 
s6="";
return s6;
}
