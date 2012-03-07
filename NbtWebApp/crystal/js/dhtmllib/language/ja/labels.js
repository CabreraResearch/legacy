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

_default="ãƒ‡ãƒ•ã‚©ãƒ«ãƒˆ"
_black="é»’"
_brown="èŒ¶"
_oliveGreen="ã‚ªãƒªãƒ¼ãƒ–ã‚°ãƒªãƒ¼ãƒ³"
_darkGreen="æ¿ƒã„ç·‘"
_darkTeal="æ¿ƒã„é’ç·‘"
_navyBlue="æ¿ƒç´º"
_indigo="ã‚¤ãƒ³ãƒ‡ã‚£ã‚´"
_darkGray="æ¿ƒã„ç°è‰²"
_darkRed="æ¿ƒã„èµ¤"
_orange="ã‚ªãƒ¬ãƒ³ã‚¸"
_darkYellow="æ¿ƒã„é»„"
_green="ç·‘"
_teal="é’ç·‘"
_blue="é’"
_blueGray="ç°é’"
_mediumGray="ç°è‰²"
_red="èµ¤"
_lightOrange="è–„ã„ã‚ªãƒ¬ãƒ³ã‚¸"
_lime="é»„ç·‘"
_seaGreen="ã‚·ãƒ¼ã‚°ãƒªãƒ¼ãƒ³"
_aqua="æ°´è‰²"
_lightBlue="æ˜Žã‚‹ã„é’"
_violet="ç´«"
_gray="ç°è‰²"
_magenta="æ¿ƒã„ãƒ”ãƒ³ã‚¯"
_gold="ã‚´ãƒ¼ãƒ«ãƒ‰"
_yellow="é»„"
_brightGreen="æ˜Žã‚‹ã„ç·‘"
_cyan="ã‚·ã‚¢ãƒ³"
_skyBlue="ã‚¹ã‚«ã‚¤ãƒ–ãƒ«ãƒ¼"
_plum="ãƒ—ãƒ©ãƒ "
_lightGray="è–„ã„ç°è‰²"
_pink="ãƒ”ãƒ³ã‚¯"
_tan="ãƒ™ãƒ¼ã‚¸ãƒ¥"
_lightYellow="è–„ã„é»„"
_lightGreen="ãƒ©ã‚¤ãƒˆã‚°ãƒªãƒ¼ãƒ³"
_lightTurquoise="ãƒ©ã‚¤ãƒˆã‚¿ãƒ¼ã‚³ã‚¤ã‚º"
_paleBlue="ãƒšãƒ¼ãƒ«ãƒ–ãƒ«ãƒ¼"
_lavender="ãƒ©ãƒ™ãƒ³ãƒ€ãƒ¼"
_white="ç™½"
_lastUsed="å‰å›žä½¿ç”¨ã•ã‚ŒãŸè‰²:"
_moreColors="ãã®ä»–ã®è‰²..."

_month=new Array

_month[0]="1 æœˆ"
_month[1]="2 æœˆ"
_month[2]="3 æœˆ"
_month[3]="4 æœˆ"
_month[4]="5 æœˆ"
_month[5]="6 æœˆ"
_month[6]="7 æœˆ"
_month[7]="8 æœˆ"
_month[8]="9 æœˆ"
_month[9]="10 æœˆ"
_month[10]="11 æœˆ"
_month[11]="12 æœˆ"

_day=new Array
_day[0]="æ—¥"
_day[1]="æœˆ"
_day[2]="ç«"
_day[3]="æ°´"
_day[4]="æœ¨"
_day[5]="é‡‘"
_day[6]="åœŸ"

_today="ä»Šæ—¥"

_AM="åˆå‰"
_PM="åˆå¾Œ"

_closeDialog="ã‚¦ã‚£ãƒ³ãƒ‰ã‚¦ã‚’é–‰ã˜ã‚‹"

_lstMoveUpLab="ä¸Šã¸ç§»å‹•"
_lstMoveDownLab="ä¸‹ã¸ç§»å‹•"
_lstMoveLeftLab="å·¦ã¸ç§»å‹•" 
_lstMoveRightLab="å³ã¸ç§»å‹•"
_lstNewNodeLab="ãƒã‚¹ãƒˆã•ã‚ŒãŸãƒ•ã‚£ãƒ«ã‚¿ã‚’è¿½åŠ "
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="é¸æŠžé …ç›®"
_lstQuickFilterLab="ã‚¯ã‚¤ãƒƒã‚¯ãƒ•ã‚£ãƒ«ã‚¿ã®è¿½åŠ "

_openMenu="{0} ã‚ªãƒ—ã‚·ãƒ§ãƒ³ã«ã‚¢ã‚¯ã‚»ã‚¹ã™ã‚‹ã«ã¯ã“ã“ã‚’ã‚¯ãƒªãƒƒã‚¯"
_openCalendarLab="ã‚«ãƒ¬ãƒ³ãƒ€ã‚’é–‹ã"

_scroll_first_tab="æœ€åˆã®ã‚¿ãƒ–ã«ã‚¹ã‚¯ãƒ­ãƒ¼ãƒ«"
_scroll_previous_tab="å‰ã®ã‚¿ãƒ–ã«ã‚¹ã‚¯ãƒ­ãƒ¼ãƒ«"
_scroll_next_tab="æ¬¡ã®ã‚¿ãƒ–ã«ã‚¹ã‚¯ãƒ­ãƒ¼ãƒ«"
_scroll_last_tab="æœ€å¾Œã®ã‚¿ãƒ–ã«ã‚¹ã‚¯ãƒ­ãƒ¼ãƒ«"

_expandedLab="å±•é–‹"
_collapsedLab="æŠ˜ã‚ŠãŸãŸã¿"
_selectedLab="é¸æŠžé …ç›®"

_expandNode="ãƒŽãƒ¼ãƒ‰ %1 ã‚’å±•é–‹"
_collapseNode="ãƒŽãƒ¼ãƒ‰ %1 ã‚’æŠ˜ã‚ŠãŸãŸã‚€"

_checkedPromptLab="è¨­å®š"
_nocheckedPromptLab="è¨­å®šã—ãªã„"
_selectionPromptLab="ã¨åŒã˜å€¤"
_noselectionPromptLab="å€¤ãªã—"

_lovTextFieldLab="ã“ã“ã«å€¤ã‚’å…¥åŠ›"
_lovCalendarLab="ã“ã“ã«æ—¥ä»˜ã‚’å…¥åŠ›"
_lovPrevChunkLab="å‰ã®ãƒãƒ£ãƒ³ã‚¯ã¸"
_lovNextChunkLab="æ¬¡ã®ãƒãƒ£ãƒ³ã‚¯ã¸"
_lovComboChunkLab="ãƒãƒ£ãƒ³ã‚¯"
_lovRefreshLab="æœ€æ–°è¡¨ç¤º"
_lovSearchFieldLab="æ¤œç´¢ã™ã‚‹ãƒ†ã‚­ã‚¹ãƒˆã‚’ã“ã“ã«å…¥åŠ›"
_lovSearchLab="æ¤œç´¢"
_lovNormalLab="æ™®é€š"
_lovMatchCase="å¤§æ–‡å­—ã¨å°æ–‡å­—ã‚’åŒºåˆ¥ã™ã‚‹"
_lovRefreshValuesLab="å€¤ã®æœ€æ–°è¡¨ç¤º"

_calendarNextMonthLab="æ¬¡ã®æœˆã¸"
_calendarPrevMonthLab="å‰ã®æœˆã¸"
_calendarNextYearLab="æ¬¡ã®å¹´ã¸"
_calendarPrevYearLab="å‰ã®å¹´ã¸"
_calendarSelectionLab="é¸æŠžã•ã‚ŒãŸæ—¥"

_menuCheckLab="ãƒã‚§ãƒƒã‚¯æ¸ˆã¿"
_menuDisableLab="ç„¡åŠ¹"
	
_level="ãƒ¬ãƒ™ãƒ«"
_closeTab="ã‚¿ãƒ–ã‚’é–‰ã˜ã‚‹"
_of=" of "

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="ãƒ˜ãƒ«ãƒ—"

_waitTitleLab="ãŠå¾…ã¡ãã ã•ã„"
_cancelButtonLab="ã‚­ãƒ£ãƒ³ã‚»ãƒ«"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="ãã®ä»–ã®ç½«ç·š..."
_bordersTooltip=new Array
_bordersTooltip[0]="å¢ƒç•Œç·šãªã—"
_bordersTooltip[1]="å·¦ã®å¢ƒç•Œç·š"
_bordersTooltip[2]="å³ã®å¢ƒç•Œç·š"
_bordersTooltip[3]="ä¸‹ã®å¢ƒç•Œç·š"
_bordersTooltip[4]="ä¸­å¤ªã®ä¸‹ç½«ç·š"
_bordersTooltip[5]="å¤ªã„ä¸‹ç½«ç·š"
_bordersTooltip[6]="ä¸Šä¸‹ã®ç½«ç·š"
_bordersTooltip[7]="ä¸Šç½«ç·šãŠã‚ˆã³ä¸­å¤ªã®ä¸‹ç½«ç·š"
_bordersTooltip[8]="ä¸Šç½«ç·šãŠã‚ˆã³å¤ªã„ä¸‹ç½«ç·š"
_bordersTooltip[9]="ã™ã¹ã¦ã®ç½«ç·š"
_bordersTooltip[10]="ã™ã¹ã¦ã®ä¸­å¤ªã®ç½«ç·š"
_bordersTooltip[11]="ã™ã¹ã¦ã®å¤ªã„ç½«ç·š"