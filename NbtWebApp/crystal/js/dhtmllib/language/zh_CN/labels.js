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

_default="é»˜è®¤å€¼"
_black="é»‘è‰²"
_brown="æ£•è‰²"
_oliveGreen="æ©„æ¦„ç»¿"
_darkGreen="æ·±ç»¿è‰²"
_darkTeal="æ·±é’è‰²"
_navyBlue="è—é’"
_indigo="é›è‰²"
_darkGray="æ·±ç°è‰²"
_darkRed="æ·±çº¢è‰²"
_orange="æ¡”é»„è‰²"
_darkYellow="æ·±é»„è‰²"
_green="ç»¿è‰²"
_teal="é’è‰²"
_blue="è“è‰²"
_blueGray="è“ç°è‰²"
_mediumGray="ä¸­ç°"
_red="çº¢è‰²"
_lightOrange="æµ…æ¡”é»„è‰²"
_lime="æµ…ç»¿è‰²"
_seaGreen="æµ·ç»¿è‰²"
_aqua="æµ…è“è‰²"
_lightBlue="æµ…è“è‰²"
_violet="ç´«ç½—å…°è‰²"
_gray="ç°è‰²"
_magenta="ç´«çº¢"
_gold="é‡‘è‰²"
_yellow="é»„è‰²"
_brightGreen="é²œç»¿è‰²"
_cyan="è“ç»¿è‰²"
_skyBlue="å¤©è“è‰²"
_plum="æ¢…çº¢è‰²"
_lightGray="æµ…ç°è‰²"
_pink="ç²‰çº¢è‰²"
_tan="æ£•è¤è‰²"
_lightYellow="æµ…é»„è‰²"
_lightGreen="æµ…ç»¿è‰²"
_lightTurquoise="æµ…é’ç»¿è‰²"
_paleBlue="æš—è“è‰²"
_lavender="æ·¡ç´«è‰²"
_white="ç™½è‰²"
_lastUsed="ä¸Šæ¬¡ä½¿ç”¨çš„é¢œè‰²ï¼š"
_moreColors="æ›´å¤šé¢œè‰²..."

_month=new Array

_month[0]="ä¸€æœˆ"
_month[1]="äºŒæœˆ"
_month[2]="ä¸‰æœˆ"
_month[3]="å››æœˆ"
_month[4]="äº”æœˆ"
_month[5]="å…­æœˆ"
_month[6]="ä¸ƒæœˆ"
_month[7]="å…«æœˆ"
_month[8]="ä¹æœˆ"
_month[9]="åæœˆ"
_month[10]="åä¸€æœˆ"
_month[11]="åäºŒæœˆ"

_day=new Array
_day[0]="æ—¥"
_day[1]="ä¸€"
_day[2]="äºŒ"
_day[3]="ä¸‰"
_day[4]="å››"
_day[5]="äº”"
_day[6]="å…­"

_today="ä»Šå¤©"

_AM="ä¸Šåˆ"
_PM="ä¸‹åˆ"

_closeDialog="å…³é—­çª—å£"

_lstMoveUpLab="å‘ä¸Šç§»åŠ¨"
_lstMoveDownLab="å‘ä¸‹ç§»åŠ¨"
_lstMoveLeftLab="å‘å·¦ç§»åŠ¨" 
_lstMoveRightLab="å‘å³ç§»åŠ¨"
_lstNewNodeLab="æ·»åŠ åµŒå¥—è¿‡æ»¤å™¨"
_lstAndLabel="å’Œ"
_lstOrLabel="æˆ–"
_lstSelectedLabel="é€‰å®š"
_lstQuickFilterLab="æ·»åŠ å¿«é€Ÿè¿‡æ»¤å™¨"

_openMenu="å•å‡»æ­¤å¤„è®¿é—®{0}é€‰é¡¹"
_openCalendarLab="æ‰“å¼€æ—¥ç¨‹è¡¨"

_scroll_first_tab="æ»šåŠ¨åˆ°ç¬¬ä¸€ä¸ªé€‰é¡¹å¡"
_scroll_previous_tab="æ»šåŠ¨åˆ°ä¸Šä¸€é€‰é¡¹å¡"
_scroll_next_tab="æ»šåŠ¨åˆ°ä¸‹ä¸€é€‰é¡¹å¡"
_scroll_last_tab="æ»šåŠ¨åˆ°æœ€åŽä¸€ä¸ªé€‰é¡¹å¡"

_expandedLab="å·²å±•å¼€"
_collapsedLab="å·²æŠ˜å "
_selectedLab="é€‰å®š"

_expandNode="å±•å¼€èŠ‚ç‚¹ %1"
_collapseNode="æŠ˜å èŠ‚ç‚¹ %1"

_checkedPromptLab="å·²è®¾ç½®"
_nocheckedPromptLab="æœªè®¾ç½®"
_selectionPromptLab="å€¼ç­‰äºŽ"
_noselectionPromptLab="æ— å€¼"

_lovTextFieldLab="åœ¨æ­¤é”®å…¥å€¼"
_lovCalendarLab="åœ¨æ­¤é”®å…¥æ—¥æœŸ"
_lovPrevChunkLab="è½¬åˆ°ä¸Šä¸€å—åŒº"
_lovNextChunkLab="è½¬åˆ°ä¸‹ä¸€å—åŒº"
_lovComboChunkLab="å—åŒº"
_lovRefreshLab="åˆ·æ–°"
_lovSearchFieldLab="åœ¨æ­¤é”®å…¥è¦æœç´¢çš„æ–‡æœ¬"
_lovSearchLab="æœç´¢"
_lovNormalLab="æ­£å¸¸"
_lovMatchCase="åŒ¹é…å¤§å°å†™"
_lovRefreshValuesLab="åˆ·æ–°å€¼"

_calendarNextMonthLab="è½¬åˆ°ä¸‹ä¸€æœˆ"
_calendarPrevMonthLab="è½¬åˆ°ä¸Šä¸€æœˆ"
_calendarNextYearLab="è½¬åˆ°ä¸‹ä¸€å¹´"
_calendarPrevYearLab="è½¬åˆ°ä¸Šä¸€å¹´"
_calendarSelectionLab="é€‰å®šæ—¥"

_menuCheckLab="å·²é€‰ä¸­"
_menuDisableLab="å·²ç¦ç”¨"
	
_level="çº§åˆ«"
_closeTab="å…³é—­é€‰é¡¹å¡"
_of="/"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="å¸®åŠ©"

_waitTitleLab="è¯·ç¨å€™"
_cancelButtonLab="å–æ¶ˆ"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="æ›´å¤šè¾¹æ¡†..."
_bordersTooltip=new Array
_bordersTooltip[0]="æ— è¾¹æ¡†"
_bordersTooltip[1]="å·¦è¾¹æ¡†"
_bordersTooltip[2]="å³è¾¹æ¡†"
_bordersTooltip[3]="ä¸‹è¾¹æ¡†"
_bordersTooltip[4]="ä¸­ç­‰å®½åº¦çš„ä¸‹è¾¹æ¡†"
_bordersTooltip[5]="ç²—ä¸‹è¾¹æ¡†"
_bordersTooltip[6]="ä¸Šä¸‹è¾¹æ¡†"
_bordersTooltip[7]="ä¸Šè¾¹æ¡†å’Œä¸­ç­‰å®½åº¦çš„ä¸‹è¾¹æ¡†"
_bordersTooltip[8]="ä¸Šè¾¹æ¡†å’Œç²—ä¸‹è¾¹æ¡†"
_bordersTooltip[9]="å››å‘¨å‡æœ‰è¾¹æ¡†"
_bordersTooltip[10]="å››å‘¨å‡æœ‰ä¸­ç­‰å®½åº¦çš„è¾¹æ¡†"
_bordersTooltip[11]="å››å‘¨å‡æœ‰ç²—è¾¹æ¡†"