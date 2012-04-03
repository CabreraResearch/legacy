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

_default="é è¨­"
_black="é»‘è‰²"
_brown="æ£•è‰²"
_oliveGreen="æ©„æ¬–ç¶ "
_darkGreen="æ·±æ©„æ¬–ç¶ "
_darkTeal="æ·±è—ç¶ "
_navyBlue="æµ·è»è—"
_indigo="é›è—è‰²"
_darkGray="æ·±ç°è‰²"
_darkRed="æ·±ç´…è‰²"
_orange="æ©™è‰²"
_darkYellow="æ·±é»ƒè‰²"
_green="ç¶ è‰²"
_teal="è—ç¶ è‰²"
_blue="è—è‰²"
_blueGray="è—ç°è‰²"
_mediumGray="ä¸­ç°è‰²"
_red="ç´…è‰²"
_lightOrange="æ·ºæ©™è‰²"
_lime="é»ƒç¶ è‰²"
_seaGreen="æµ·è—»ç¶ "
_aqua="æ°´è—è‰²"
_lightBlue="æ·ºè—è‰²"
_violet="ç´«è‰²"
_gray="ç°è‰²"
_magenta="æ´‹ç´…è‰²"
_gold="é‡‘è‰²"
_yellow="é»ƒè‰²"
_brightGreen="äº®ç¶ è‰²"
_cyan="é’è‰²"
_skyBlue="å¤©è—è‰²"
_plum="æ¢…ç´…è‰²"
_lightGray="æ·ºç°è‰²"
_pink="ç²‰ç´…è‰²"
_tan="é»ƒè¤è‰²"
_lightYellow="æ·ºé»ƒè‰²"
_lightGreen="æ·ºç¶ è‰²"
_lightTurquoise="æ·ºç¶ è—"
_paleBlue="æ·¡è—è‰²"
_lavender="æ·¡ç´«è‰²"
_white="ç™½è‰²"
_lastUsed="ä¸Šä¸€æ¬¡ä½¿ç”¨:"
_moreColors="å…¶ä»–è‰²å½©..."

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
_day[0]="é€±æ—¥"
_day[1]="é€±ä¸€"
_day[2]="é€±äºŒ"
_day[3]="é€±ä¸‰"
_day[4]="é€±å››"
_day[5]="é€±äº”"
_day[6]="é€±å…­"

_today="ä»Šå¤©"

_AM="ä¸Šåˆ"
_PM="ä¸‹åˆ"

_closeDialog="é—œé–‰è¦–çª—"

_lstMoveUpLab="å¾€ä¸Šç§»å‹•"
_lstMoveDownLab="å¾€ä¸‹ç§»å‹•"
_lstMoveLeftLab="å¾€å·¦ç§»å‹•" 
_lstMoveRightLab="å¾€å³ç§»å‹•"
_lstNewNodeLab="æ–°å¢žå·¢ç‹€ç¯©é¸å™¨"
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="å·²é¸å–"
_lstQuickFilterLab="æ–°å¢žå¿«é€Ÿç¯©é¸å™¨"

_openMenu="æŒ‰ä¸€ä¸‹é€™è£¡ä»¥å­˜å–{0}é¸é …"
_openCalendarLab="é–‹å•Ÿæ—¥æ›†"

_scroll_first_tab="æ²å‹•è‡³ç¬¬ä¸€å€‹æ¨™ç±¤"
_scroll_previous_tab="æ²å‹•è‡³ä¸Šä¸€å€‹æ¨™ç±¤"
_scroll_next_tab="æ²å‹•è‡³ä¸‹ä¸€å€‹æ¨™ç±¤"
_scroll_last_tab="æ²å‹•è‡³æœ€å¾Œä¸€å€‹æ¨™ç±¤"

_expandedLab="å±•é–‹çš„"
_collapsedLab="æ”¶åˆçš„"
_selectedLab="å·²é¸å–"

_expandNode="å±•é–‹ç¯€é»ž %1"
_collapseNode="æ”¶åˆç¯€é»ž %1"

_checkedPromptLab="å·²è¨­å®š"
_nocheckedPromptLab="æœªè¨­å®š"
_selectionPromptLab="å€¼ç­‰æ–¼"
_noselectionPromptLab="æ²’æœ‰å€¼"

_lovTextFieldLab="åœ¨æ­¤è¼¸å…¥å€¼"
_lovCalendarLab="åœ¨æ­¤è¼¸å…¥æ—¥æœŸ"
_lovPrevChunkLab="ç§»è‡³ä¸Šä¸€å€‹å€å¡Š"
_lovNextChunkLab="ç§»è‡³ä¸‹ä¸€å€‹å€å¡Š"
_lovComboChunkLab="å€å¡Š"
_lovRefreshLab="é‡æ–°æ•´ç†"
_lovSearchFieldLab="åœ¨æ­¤è¼¸å…¥è¦æœå°‹çš„æ–‡å­—"
_lovSearchLab="æœå°‹"
_lovNormalLab="ä¸€èˆ¬"
_lovMatchCase="å¤§å°å¯«é ˆç›¸ç¬¦"
_lovRefreshValuesLab="é‡æ–°æ•´ç†å€¼"

_calendarNextMonthLab="ç§»è‡³ä¸‹å€‹æœˆ"
_calendarPrevMonthLab="ç§»è‡³ä¸Šå€‹æœˆ"
_calendarNextYearLab="ç§»è‡³å¾Œä¸€å¹´"
_calendarPrevYearLab="ç§»è‡³å‰ä¸€å¹´"
_calendarSelectionLab="é¸å–çš„æ—¥ "

_menuCheckLab="å·²æª¢æŸ¥"
_menuDisableLab="å·²åœç”¨"
	
_level="å±¤ç´š"
_closeTab="é—œé–‰ç´¢å¼•æ¨™ç±¤"
_of="/"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="èªªæ˜Ž"

_waitTitleLab="è«‹ç¨å€™"
_cancelButtonLab="å–æ¶ˆ"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="å…¶ä»–æ¡†ç·š..."
_bordersTooltip=new Array
_bordersTooltip[0]="ç„¡æ¡†ç·š"
_bordersTooltip[1]="å·¦æ¡†ç·š"
_bordersTooltip[2]="å³æ¡†ç·š"
_bordersTooltip[3]="ä¸‹æ¡†ç·š"
_bordersTooltip[4]="é©ä¸­ä¸‹æ¡†ç·š"
_bordersTooltip[5]="ç²—ä¸‹æ¡†ç·š"
_bordersTooltip[6]="ä¸Šä¸‹æ¡†ç·š"
_bordersTooltip[7]="ä¸Šæ–¹å’Œé©ä¸­ä¸‹æ¡†ç·š"
_bordersTooltip[8]="ä¸Šæ–¹å’Œç²—ä¸‹æ¡†ç·š"
_bordersTooltip[9]="æ‰€æœ‰æ¡†ç·š"
_bordersTooltip[10]="æ‰€æœ‰é©ä¸­æ¡†ç·š"
_bordersTooltip[11]="æ‰€æœ‰ç²—æ¡†ç·š"