// <script>
/*
=============================================================
WebIntelligence(r) Report Panel
Copyright(c) 2001-2003 Business Objects S.A.
All rights reserved

Use and support of this software is governed by the terms
and conditions of the software license agreement and support
policy of Business Objects S.A. and/or its subsidiaries. 
The Business Objects products and technology are protected
by the US patent number 5,555,403 and 6,247,008

File: labels.js


=============================================================
*/

_default="ê¸°ë³¸ê°’"
_black="ê²€ì •"
_brown="ë°¤ìƒ‰"
_oliveGreen="í™©ë¡ìƒ‰"
_darkGreen="ì§„í•œ ë…¹ìƒ‰"
_darkTeal="ì§„í•œ ì²­ë¡"
_navyBlue="ì§™ì€ íŒŒëž‘"
_indigo="ë‚¨ìƒ‰"
_darkGray="ì§„í•œ íšŒìƒ‰"
_darkRed="ì§„í•œ ë¹¨ê°•"
_orange="ì£¼í™©"
_darkYellow="ì§„í•œ ë…¸ëž‘"
_green="ë…¹ìƒ‰"
_teal="ì²­ë¡"
_blue="íŒŒëž‘"
_blueGray="ì²­íšŒìƒ‰"
_mediumGray="ì¤‘ê°„ íšŒìƒ‰"
_red="ë¹¨ê°•"
_lightOrange="ì—°í•œ ì£¼í™©"
_lime="ë¼ìž„ìƒ‰"
_seaGreen="í•´ë¡ìƒ‰"
_aqua="ë°”ë‹¤ìƒ‰"
_lightBlue="ì—°í•œ íŒŒëž‘"
_violet="ë³´ë¼"
_gray="íšŒìƒ‰"
_magenta="ìží™"
_gold="í™©ê¸ˆìƒ‰"
_yellow="ë…¸ëž‘"
_brightGreen="ë°ì€ ë…¹ìƒ‰"
_cyan="ë…¹ì²­"
_skyBlue="í•˜ëŠ˜ìƒ‰"
_plum="ì§„í•œ ë³´ë¼"
_lightGray="ì—°í•œ íšŒìƒ‰"
_pink="ë¶„í™"
_tan="í™©ê°ˆìƒ‰"
_lightYellow="ì—°í•œ ë…¸ëž‘"
_lightGreen="ì—°í•œ ë…¹ìƒ‰"
_lightTurquoise="ì—°í•œ ì˜¥ìƒ‰"
_paleBlue="íë¦° íŒŒëž‘"
_lavender="ë¼ë²¤ë”ìƒ‰"
_white="í°ìƒ‰"
_lastUsed="ë§ˆì§€ë§‰ ì‚¬ìš©:"
_moreColors="ë‹¤ë¥¸ ìƒ‰..."

_month=new Array

_month[0]="ï¼‘ì›”"
_month[1]="ï¼’ì›”"
_month[2]="ï¼“ì›”"
_month[3]="ï¼”ì›”"
_month[4]="ï¼•ì›”"
_month[5]="ï¼–ì›”"
_month[6]="ï¼—ì›”"
_month[7]="ï¼˜ì›”"
_month[8]="ï¼™ì›”"
_month[9]="10ì›”"
_month[10]="11ì›”"
_month[11]="12ì›”"

_day=new Array
_day[0]="S"
_day[1]="M"
_day[2]="T"
_day[3]="W"
_day[4]="T"
_day[5]="F"
_day[6]="S"

_today="ì˜¤ëŠ˜"

_AM="ì˜¤ì „"
_PM="ì˜¤í›„"

_closeDialog="ì°½ ë‹«ê¸°"

_lstMoveUpLab="ìœ„ë¡œ ì´ë™"
_lstMoveDownLab="ì•„ëž˜ë¡œ ì´ë™"
_lstMoveLeftLab="ì™¼ìª½ìœ¼ë¡œ ì´ë™" 
_lstMoveRightLab="ì˜¤ë¥¸ìª½ìœ¼ë¡œ ì´ë™"
_lstNewNodeLab="ì¤‘ì²©ëœ í•„í„° ì¶”ê°€"
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="ì„ íƒë¨"
_lstQuickFilterLab="ë¹ ë¥¸ í•„í„° ì¶”ê°€"

_openMenu="{0} ì˜µì…˜ì— ì•¡ì„¸ìŠ¤í•˜ë ¤ë©´ ì—¬ê¸°ë¥¼ í´ë¦­í•˜ì‹­ì‹œì˜¤."
_openCalendarLab="ë‹¬ë ¥ ì—´ê¸°"

_scroll_first_tab="ì²« ë²ˆì§¸ íƒ­ìœ¼ë¡œ ìŠ¤í¬ë¡¤"
_scroll_previous_tab="ì´ì „ íƒ­ìœ¼ë¡œ ìŠ¤í¬ë¡¤"
_scroll_next_tab="ë‹¤ìŒ íƒ­ìœ¼ë¡œ ìŠ¤í¬ë¡¤"
_scroll_last_tab="ë§ˆì§€ë§‰ íƒ­ìœ¼ë¡œ ìŠ¤í¬ë¡¤"

_expandedLab="í™•ìž¥ë¨"
_collapsedLab="ì¶•ì†Œë¨"
_selectedLab="ì„ íƒë¨"

_expandNode="%1 ë…¸ë“œ í™•ìž¥"
_collapseNode="%1 ë…¸ë“œ ì¶•ì†Œ"

_checkedPromptLab="ì„¤ì •"
_nocheckedPromptLab="ì„¤ì • ì•ˆ í•¨"
_selectionPromptLab="ê°’ì´ ë‹¤ìŒê³¼ ê°™ìŒ"
_noselectionPromptLab="ê°’ ì—†ìŒ"

_lovTextFieldLab="ì—¬ê¸°ì— ê°’ ìž…ë ¥"
_lovCalendarLab="ì—¬ê¸°ì— ë‚ ì§œ ìž…ë ¥"
_lovPrevChunkLab="ì´ì „ ì²­í¬ë¡œ ì´ë™"
_lovNextChunkLab="ë‹¤ìŒ ì²­í¬ë¡œ ì´ë™"
_lovComboChunkLab="ì²­í¬"
_lovRefreshLab="ìƒˆë¡œ ê³ ì¹¨"
_lovSearchFieldLab="ì—¬ê¸°ì— ê²€ìƒ‰í•  í…ìŠ¤íŠ¸ ìž…ë ¥"
_lovSearchLab="ê²€ìƒ‰"
_lovNormalLab="ë³´í†µ"
_lovMatchCase="ëŒ€/ì†Œë¬¸ìž êµ¬ë¶„"
_lovRefreshValuesLab="ê°’ ìƒˆë¡œ ê³ ì¹¨"

_calendarNextMonthLab="ë‹¤ìŒ ë‹¬ë¡œ ì´ë™"
_calendarPrevMonthLab="ì´ì „ ë‹¬ë¡œ ì´ë™"
_calendarNextYearLab="ë‹¤ìŒ ì—°ë„ë¡œ ì´ë™"
_calendarPrevYearLab="ì´ì „ ì—°ë„ë¡œ ì´ë™"
_calendarSelectionLab="ì„ íƒí•œ ë‚ ì§œ"

_menuCheckLab="ì„ íƒë¨"
_menuDisableLab="ì‚¬ìš© ì•ˆ í•¨"
	
_level="ìˆ˜ì¤€"
_closeTab="íƒ­ ë‹«ê¸°"
_of=" /"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="ë„ì›€ë§"

_waitTitleLab="ìž ì‹œ ê¸°ë‹¤ë ¤ ì£¼ì‹­ì‹œì˜¤."
_cancelButtonLab="ì·¨ì†Œ"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="ê¸°íƒ€ í…Œë‘ë¦¬..."
_bordersTooltip=new Array
_bordersTooltip[0]="í…Œë‘ë¦¬ ì—†ìŒ"
_bordersTooltip[1]="ì™¼ìª½ í…Œë‘ë¦¬"
_bordersTooltip[2]="ì˜¤ë¥¸ìª½ í…Œë‘ë¦¬"
_bordersTooltip[3]="ì•„ëž˜ìª½ í…Œë‘ë¦¬"
_bordersTooltip[4]="ì¤‘ê°„ ì•„ëž˜ìª½ í…Œë‘ë¦¬"
_bordersTooltip[5]="ë‘êº¼ìš´ ì•„ëž˜ìª½ í…Œë‘ë¦¬"
_bordersTooltip[6]="ìœ„ìª½ ë° ì•„ëž˜ìª½ í…Œë‘ë¦¬"
_bordersTooltip[7]="ìœ„ìª½ ë° ì¤‘ê°„ ì•„ëž˜ìª½ í…Œë‘ë¦¬"
_bordersTooltip[8]="ìœ„ìª½ ë° ë‘êº¼ìš´ ì•„ëž˜ìª½ í…Œë‘ë¦¬"
_bordersTooltip[9]="ëª¨ë“  í…Œë‘ë¦¬"
_bordersTooltip[10]="ëª¨ë“  ì¤‘ê°„ í…Œë‘ë¦¬"
_bordersTooltip[11]="ëª¨ë“  ë‘êº¼ìš´ í…Œë‘ë¦¬"