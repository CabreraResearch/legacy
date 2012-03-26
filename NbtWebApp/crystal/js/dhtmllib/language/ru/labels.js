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

_default="ÐŸÐ¾ ÑƒÐ¼Ð¾Ð»Ñ‡Ð°Ð½Ð¸ÑŽ"
_black="Ð§ÐµÑ€Ð½Ñ‹Ð¹"
_brown="ÐšÐ¾Ñ€Ð¸Ñ‡Ð½ÐµÐ²Ñ‹Ð¹"
_oliveGreen="ÐžÐ»Ð¸Ð²ÐºÐ¾Ð²Ð¾-Ð·ÐµÐ»ÐµÐ½Ñ‹Ð¹"
_darkGreen="Ð¢ÐµÐ¼Ð½Ð¾-Ð·ÐµÐ»ÐµÐ½Ñ‹Ð¹"
_darkTeal="Ð¢ÐµÐ¼Ð½Ð¾-Ð±Ð¸Ñ€ÑŽÐ·Ð¾Ð²Ñ‹Ð¹"
_navyBlue="Ð¢ÐµÐ¼Ð½Ð¾-ÑÐ¸Ð½Ð¸Ð¹"
_indigo="Ð˜Ð½Ð´Ð¸Ð³Ð¾"
_darkGray="Ð¢ÐµÐ¼Ð½Ð¾-ÑÐµÑ€Ñ‹Ð¹"
_darkRed="Ð¢ÐµÐ¼Ð½Ð¾-ÐºÑ€Ð°ÑÐ½Ñ‹Ð¹"
_orange="ÐžÑ€Ð°Ð½Ð¶ÐµÐ²Ñ‹Ð¹"
_darkYellow="Ð¢ÐµÐ¼Ð½Ð¾-Ð¶ÐµÐ»Ñ‚Ñ‹Ð¹"
_green="Ð—ÐµÐ»ÐµÐ½Ñ‹Ð¹"
_teal="Ð‘Ð¸Ñ€ÑŽÐ·Ð¾Ð²Ñ‹Ð¹"
_blue="Ð¡Ð¸Ð½Ð¸Ð¹"
_blueGray="Ð¡ÐµÑ€Ð¾-ÑÐ¸Ð½Ð¸Ð¹"
_mediumGray="Ð¡Ñ€ÐµÐ´Ð½Ðµ-ÑÐµÑ€Ñ‹Ð¹"
_red="ÐšÑ€Ð°ÑÐ½Ñ‹Ð¹"
_lightOrange="Ð¡Ð²ÐµÑ‚Ð»Ð¾-Ð¾Ñ€Ð°Ð½Ð¶ÐµÐ²Ñ‹Ð¹"
_lime="Ð˜Ð·Ð²ÐµÑÑ‚ÐºÐ¾Ð²Ñ‹Ð¹"
_seaGreen="ÐœÐ¾Ñ€ÑÐºÐ°Ñ Ð²Ð¾Ð»Ð½Ð°"
_aqua="ÐÐºÐ²Ð°Ð¼Ð°Ñ€Ð¸Ð½"
_lightBlue="Ð¡Ð²ÐµÑ‚Ð»Ð¾-ÑÐ¸Ð½Ð¸Ð¹"
_violet="Ð¤Ð¸Ð¾Ð»ÐµÑ‚Ð¾Ð²Ñ‹Ð¹"
_gray="Ð¡ÐµÑ€Ñ‹Ð¹"
_magenta="ÐŸÑƒÑ€Ð¿ÑƒÑ€Ð½Ñ‹Ð¹"
_gold="Ð—Ð¾Ð»Ð¾Ñ‚Ð¾Ð¹"
_yellow="Ð–ÐµÐ»Ñ‚Ñ‹Ð¹"
_brightGreen="Ð¯Ñ€ÐºÐ¾-Ð·ÐµÐ»ÐµÐ½Ñ‹Ð¹"
_cyan="Ð“Ð¾Ð»ÑƒÐ±Ð¾Ð¹"
_skyBlue="ÐÐµÐ±ÐµÑÐ½Ð¾-Ð³Ð¾Ð»ÑƒÐ±Ð¾Ð¹"
_plum="Ð¢ÐµÐ¼Ð½Ð¾-Ñ„Ð¸Ð¾Ð»ÐµÑ‚Ð¾Ð²Ñ‹Ð¹"
_lightGray="Ð¡Ð²ÐµÑ‚Ð»Ð¾-ÑÐµÑ€Ñ‹Ð¹"
_pink="Ð Ð¾Ð·Ð¾Ð²Ñ‹Ð¹"
_tan="Ð–ÐµÐ»Ñ‚Ð¾-ÐºÐ¾Ñ€Ð¸Ñ‡Ð½ÐµÐ²Ñ‹Ð¹"
_lightYellow="Ð¡Ð²ÐµÑ‚Ð»Ð¾-Ð¶ÐµÐ»Ñ‚Ñ‹Ð¹"
_lightGreen="Ð¡Ð²ÐµÑ‚Ð»Ð¾-Ð·ÐµÐ»ÐµÐ½Ñ‹Ð¹"
_lightTurquoise="Ð¡Ð²ÐµÑ‚Ð»Ð¾-Ð±Ð¸Ñ€ÑŽÐ·Ð¾Ð²Ñ‹Ð¹"
_paleBlue="Ð‘Ð»ÐµÐ´Ð½Ð¾-Ð³Ð¾Ð»ÑƒÐ±Ð¾Ð¹"
_lavender="Ð‘Ð»ÐµÐ´Ð½Ð¾-Ð»Ð¸Ð»Ð¾Ð²Ñ‹Ð¹"
_white="Ð‘ÐµÐ»Ñ‹Ð¹"
_lastUsed="ÐŸÐ¾ÑÐ»ÐµÐ´Ð½Ð¸Ð¹ Ð¸ÑÐ¿Ð¾Ð»ÑŒÐ·ÑƒÐµÐ¼Ñ‹Ð¹:"
_moreColors="Ð”Ð¾Ð¿Ð¾Ð»Ð½Ð¸Ñ‚ÐµÐ»ÑŒÐ½Ñ‹Ðµ Ñ†Ð²ÐµÑ‚Ð°..."

_month=new Array

_month[0]="Ð¯Ð½Ð²Ð°Ñ€ÑŒ"
_month[1]="Ð¤ÐµÐ²Ñ€Ð°Ð»ÑŒ"
_month[2]="ÐœÐ°Ñ€Ñ‚"
_month[3]="ÐÐ¿Ñ€ÐµÐ»ÑŒ"
_month[4]="ÐœÐ°Ð¹"
_month[5]="Ð˜ÑŽÐ½ÑŒ"
_month[6]="Ð˜ÑŽÐ»ÑŒ"
_month[7]="ÐÐ²Ð³ÑƒÑÑ‚"
_month[8]="Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€ÑŒ"
_month[9]="ÐžÐºÑ‚ÑÐ±Ñ€ÑŒ"
_month[10]="ÐÐ¾ÑÐ±Ñ€ÑŒ"
_month[11]="Ð”ÐµÐºÐ°Ð±Ñ€ÑŒ"

_day=new Array
_day[0]="Ð’"
_day[1]="ÐŸ"
_day[2]="Ð’"
_day[3]="Ð¡"
_day[4]="Ð§"
_day[5]="ÐŸ"
_day[6]="Ð¡"

_today="Ð¡ÐµÐ³Ð¾Ð´Ð½Ñ"

_AM="AM"
_PM="PM"

_closeDialog="Ð—Ð°ÐºÑ€Ñ‹Ñ‚ÑŒ Ð¾ÐºÐ½Ð¾"

_lstMoveUpLab="ÐŸÐµÑ€ÐµÐ¼ÐµÑÑ‚Ð¸Ñ‚ÑŒ Ð²Ð²ÐµÑ€Ñ…"
_lstMoveDownLab="ÐŸÐµÑ€ÐµÐ¼ÐµÑÑ‚Ð¸Ñ‚ÑŒ Ð²Ð½Ð¸Ð·"
_lstMoveLeftLab="ÐŸÐµÑ€ÐµÐ¼ÐµÑÑ‚Ð¸Ñ‚ÑŒ Ð²Ð»ÐµÐ²Ð¾" 
_lstMoveRightLab="ÐŸÐµÑ€ÐµÐ¼ÐµÑÑ‚Ð¸Ñ‚ÑŒ Ð²Ð¿Ñ€Ð°Ð²Ð¾"
_lstNewNodeLab="Ð”Ð¾Ð±Ð°Ð²Ð¸Ñ‚ÑŒ Ð²Ð»Ð¾Ð¶ÐµÐ½Ð½Ñ‹Ð¹ Ñ„Ð¸Ð»ÑŒÑ‚Ñ€"
_lstAndLabel="Ð¸"
_lstOrLabel="Ð˜Ð›Ð˜"
_lstSelectedLabel="Ð’Ñ‹Ð±Ð¾Ñ€ Ð²Ñ‹Ð¿Ð¾Ð»Ð½ÐµÐ½"
_lstQuickFilterLab="Ð”Ð¾Ð±Ð°Ð²Ð¸Ñ‚ÑŒ Ð±Ñ‹ÑÑ‚Ñ€Ñ‹Ð¹ Ñ„Ð¸Ð»ÑŒÑ‚Ñ€"

_openMenu="Ð”Ð»Ñ Ð¿Ð¾Ð»ÑƒÑ‡ÐµÐ½Ð¸Ñ Ð´Ð¾ÑÑ‚ÑƒÐ¿Ð° Ðº Ð¿Ð°Ñ€Ð°Ð¼ÐµÑ‚Ñ€Ð°Ð¼ {0} Ñ‰ÐµÐ»ÐºÐ½Ð¸Ñ‚Ðµ Ð·Ð´ÐµÑÑŒ"
_openCalendarLab="ÐžÑ‚ÐºÑ€Ñ‹Ñ‚ÑŒ ÐºÐ°Ð»ÐµÐ½Ð´Ð°Ñ€ÑŒ"

_scroll_first_tab="ÐŸÑ€Ð¾ÐºÑ€ÑƒÑ‚Ð¸Ñ‚ÑŒ Ð´Ð¾ Ð¿ÐµÑ€Ð²Ð¾Ð¹ Ð²ÐºÐ»Ð°Ð´ÐºÐ¸"
_scroll_previous_tab="ÐŸÑ€Ð¾ÐºÑ€ÑƒÑ‚Ð¸Ñ‚ÑŒ Ð´Ð¾ Ð¿Ñ€ÐµÐ´Ñ‹Ð´ÑƒÑ‰ÐµÐ¹ Ð²ÐºÐ»Ð°Ð´ÐºÐ¸"
_scroll_next_tab="ÐŸÑ€Ð¾ÐºÑ€ÑƒÑ‚Ð¸Ñ‚ÑŒ Ð´Ð¾ ÑÐ»ÐµÐ´ÑƒÑŽÑ‰ÐµÐ¹ Ð²ÐºÐ»Ð°Ð´ÐºÐ¸"
_scroll_last_tab="ÐŸÑ€Ð¾ÐºÑ€ÑƒÑ‚Ð¸Ñ‚ÑŒ Ð´Ð¾ Ð¿Ð¾ÑÐ»ÐµÐ´Ð½ÐµÐ¹ Ð²ÐºÐ»Ð°Ð´ÐºÐ¸"

_expandedLab="Ð Ð°Ð·Ð²ÐµÑ€Ð½ÑƒÑ‚Ð¾"
_collapsedLab="Ð¡Ð²ÐµÑ€Ð½ÑƒÑ‚Ð¾"
_selectedLab="Ð’Ñ‹Ð±Ð¾Ñ€ Ð²Ñ‹Ð¿Ð¾Ð»Ð½ÐµÐ½"

_expandNode="Ð Ð°Ð·Ð²ÐµÑ€Ð½ÑƒÑ‚ÑŒ ÑƒÐ·ÐµÐ» %1"
_collapseNode="Ð¡Ð²ÐµÑ€Ð½ÑƒÑ‚ÑŒ ÑƒÐ·ÐµÐ» %1"

_checkedPromptLab="Ð—Ð°Ð´Ð°Ð½Ð¾"
_nocheckedPromptLab="ÐÐµ Ð·Ð°Ð´Ð°Ð½Ð¾"
_selectionPromptLab="Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ñ, Ñ€Ð°Ð²Ð½Ñ‹Ðµ"
_noselectionPromptLab="ÐžÑ‚ÑÑƒÑ‚ÑÑ‚Ð²ÑƒÑŽÑ‚ Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ñ"

_lovTextFieldLab="Ð’Ð²ÐµÐ´Ð¸Ñ‚Ðµ Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ñ Ð·Ð´ÐµÑÑŒ"
_lovCalendarLab="Ð’Ð²ÐµÐ´Ð¸Ñ‚Ðµ Ð´Ð°Ñ‚Ñƒ Ð·Ð´ÐµÑÑŒ"
_lovPrevChunkLab="ÐŸÐµÑ€ÐµÐ¹Ñ‚Ð¸ Ðº Ð¿Ñ€ÐµÐ´Ñ‹Ð´ÑƒÑ‰ÐµÐ¼Ñƒ Ð±Ð»Ð¾ÐºÑƒ"
_lovNextChunkLab="ÐŸÐµÑ€ÐµÐ¹Ñ‚Ð¸ Ðº ÑÐ»ÐµÐ´ÑƒÑŽÑ‰ÐµÐ¼Ñƒ Ð±Ð»Ð¾ÐºÑƒ"
_lovComboChunkLab="Ð‘Ð»Ð¾Ðº"
_lovRefreshLab="ÐžÐ±Ð½Ð¾Ð²Ð¸Ñ‚ÑŒ Ð½Ð° ÑÐµÑ€Ð²ÐµÑ€Ðµ"
_lovSearchFieldLab="Ð’Ð²ÐµÐ´Ð¸Ñ‚Ðµ Ð¸ÑÐºÐ¾Ð¼Ñ‹Ð¹ Ñ‚ÐµÐºÑÑ‚ Ð·Ð´ÐµÑÑŒ"
_lovSearchLab="ÐŸÐ¾Ð¸ÑÐº"
_lovNormalLab="ÐžÐ±Ñ‹Ñ‡Ð½Ñ‹Ð¹"
_lovMatchCase="Ð£Ñ‡Ð¸Ñ‚Ñ‹Ð²Ð°Ñ‚ÑŒ Ñ€ÐµÐ³Ð¸ÑÑ‚Ñ€"
_lovRefreshValuesLab="ÐžÐ±Ð½Ð¾Ð²Ð¸Ñ‚ÑŒ Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ñ"

_calendarNextMonthLab="ÐŸÐµÑ€ÐµÐ¹Ñ‚Ð¸ Ðº ÑÐ»ÐµÐ´ÑƒÑŽÑ‰ÐµÐ¼Ñƒ Ð¼ÐµÑÑÑ†Ñƒ"
_calendarPrevMonthLab="ÐŸÐµÑ€ÐµÐ¹Ñ‚Ð¸ Ðº Ð¿Ñ€ÐµÐ´Ñ‹Ð´ÑƒÑ‰ÐµÐ¼Ñƒ Ð¼ÐµÑÑÑ†Ñƒ"
_calendarNextYearLab="ÐŸÐµÑ€ÐµÐ¹Ñ‚Ð¸ Ðº ÑÐ»ÐµÐ´ÑƒÑŽÑ‰ÐµÐ¼Ñƒ Ð³Ð¾Ð´Ñƒ"
_calendarPrevYearLab="ÐŸÐµÑ€ÐµÐ¹Ñ‚Ð¸ Ðº Ð¿Ñ€ÐµÐ´Ñ‹Ð´ÑƒÑ‰ÐµÐ¼Ñƒ Ð³Ð¾Ð´Ñƒ"
_calendarSelectionLab="Ð’Ñ‹Ð±Ñ€Ð°Ð½Ð½Ñ‹Ð¹ Ð´ÐµÐ½ÑŒ"

_menuCheckLab="ÐžÑ‚Ð¼ÐµÑ‡ÐµÐ½"
_menuDisableLab="ÐžÑ‚ÐºÐ»ÑŽÑ‡ÐµÐ½"
	
_level="Ð£Ñ€Ð¾Ð²ÐµÐ½ÑŒ"
_closeTab="Ð—Ð°ÐºÑ€Ñ‹Ñ‚ÑŒ Ð²ÐºÐ»Ð°Ð´ÐºÑƒ"
_of="Ð¸Ð·"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="Ð¡Ð¿Ñ€Ð°Ð²ÐºÐ°"

_waitTitleLab="ÐŸÐ¾Ð´Ð¾Ð¶Ð´Ð¸Ñ‚Ðµ"
_cancelButtonLab="ÐžÑ‚Ð¼ÐµÐ½Ð°"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Ð”Ð¾Ð¿Ð¾Ð»Ð½Ð¸Ñ‚ÐµÐ»ÑŒÐ½Ñ‹Ðµ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ñ‹..."
_bordersTooltip=new Array
_bordersTooltip[0]="Ð‘ÐµÐ· Ð³Ñ€Ð°Ð½Ð¸Ñ†Ñ‹"
_bordersTooltip[1]="Ð›ÐµÐ²Ð°Ñ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð°"
_bordersTooltip[2]="ÐŸÑ€Ð°Ð²Ð°Ñ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð°"
_bordersTooltip[3]="ÐÐ¸Ð¶Ð½ÑÑ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð°"
_bordersTooltip[4]="ÐÐ¸Ð¶Ð½ÑÑ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð° ÑÑ€ÐµÐ´Ð½ÐµÐ¹ Ñ‚Ð¾Ð»Ñ‰Ð¸Ð½Ñ‹"
_bordersTooltip[5]="Ð¢Ð¾Ð»ÑÑ‚Ð°Ñ Ð½Ð¸Ð¶Ð½ÑÑ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð°"
_bordersTooltip[6]="Ð’ÐµÑ€Ñ…Ð½ÑÑ Ð¸ Ð½Ð¸Ð¶Ð½ÑÑ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð°"
_bordersTooltip[7]="Ð’ÐµÑ€Ñ…Ð½ÑÑ Ð¸ Ð½Ð¸Ð¶Ð½ÑÑ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð° ÑÑ€ÐµÐ´Ð½ÐµÐ¹ Ñ‚Ð¾Ð»Ñ‰Ð¸Ð½Ñ‹"
_bordersTooltip[8]="Ð’ÐµÑ€Ñ…Ð½ÑÑ Ð¸ Ñ‚Ð¾Ð»ÑÑ‚Ð°Ñ Ð½Ð¸Ð¶Ð½ÑÑ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ð°"
_bordersTooltip[9]="Ð’ÑÐµ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ñ‹"
_bordersTooltip[10]="Ð’ÑÐµ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ñ‹ ÑÑ€ÐµÐ´Ð½ÐµÐ¹ Ñ‚Ð¾Ð»Ñ‰Ð¸Ð½Ñ‹"
_bordersTooltip[11]="Ð’ÑÐµ Ñ‚Ð¾Ð»ÑÑ‚Ñ‹Ðµ Ð³Ñ€Ð°Ð½Ð¸Ñ†Ñ‹"