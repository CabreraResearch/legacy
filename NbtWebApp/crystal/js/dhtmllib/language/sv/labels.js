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

_default="Standard"
_black="Svart"
_brown="Brun"
_oliveGreen="OlivgrÃ¶n"
_darkGreen="MÃ¶rkgrÃ¶n"
_darkTeal="MÃ¶rk blÃ¥grÃ¶n"
_navyBlue="MarinblÃ¥"
_indigo="Indigo"
_darkGray="MÃ¶rkgrÃ¥"
_darkRed="MÃ¶rkrÃ¶d"
_orange="Orange"
_darkYellow="MÃ¶rkgul"
_green="GrÃ¶n"
_teal="BlÃ¥grÃ¶n"
_blue="BlÃ¥"
_blueGray="BlÃ¥grÃ¥"
_mediumGray="MellangrÃ¥"
_red="RÃ¶d"
_lightOrange="Ljus orange"
_lime="Lime"
_seaGreen="HavsgrÃ¶n"
_aqua="Akvamarin"
_lightBlue="LjusblÃ¥"
_violet="Lila"
_gray="GrÃ¥"
_magenta="Magenta"
_gold="Guld"
_yellow="Gul"
_brightGreen="LjusgrÃ¶n"
_cyan="CyanblÃ¥"
_skyBlue="HimmelsblÃ¥"
_plum="Lila"
_lightGray="LjusgrÃ¥"
_pink="Rosa"
_tan="Mellanbrun"
_lightYellow="Ljusgul"
_lightGreen="LjusgrÃ¶n"
_lightTurquoise="Ljusturkos"
_paleBlue="LjusblÃ¥"
_lavender="Lavendel"
_white="Vit"
_lastUsed="AnvÃ¤ndes senast:"
_moreColors="Fler fÃ¤rger..."

_month=new Array

_month[0]="JANUARI"
_month[1]="FEBRUARI"
_month[2]="MARS"
_month[3]="APRIL"
_month[4]="MAJ"
_month[5]="JUNI"
_month[6]="JULI"
_month[7]="AUGUSTI"
_month[8]="SEPTEMBER"
_month[9]="OKTOBER"
_month[10]="NOVEMBER"
_month[11]="DECEMBER"

_day=new Array
_day[0]="S"
_day[1]="M"
_day[2]="T"
_day[3]="O"
_day[4]="T"
_day[5]="F"
_day[6]="L"

_today="I dag"

_AM="FM"
_PM="EM"

_closeDialog="StÃ¤ng fÃ¶nster"

_lstMoveUpLab="Flytta upp"
_lstMoveDownLab="Flytta ned"
_lstMoveLeftLab="Flytta vÃ¤nster" 
_lstMoveRightLab="Flytta hÃ¶ger"
_lstNewNodeLab="LÃ¤gg till kapslat filter"
_lstAndLabel="OCH"
_lstOrLabel="ELLER"
_lstSelectedLabel="Markerad"
_lstQuickFilterLab="LÃ¤gg till snabbfilter"

_openMenu="Klicka hÃ¤r fÃ¶r att fÃ¥ tillgÃ¥ng till {0} alternativ"
_openCalendarLab="Ã–ppna kalender"

_scroll_first_tab="BlÃ¤ddra till fÃ¶rsta fliken"
_scroll_previous_tab="BlÃ¤ddra till fÃ¶regÃ¥ende flik"
_scroll_next_tab="BlÃ¤ddra till nÃ¤sta flik"
_scroll_last_tab="BlÃ¤ddra till sista fliken"

_expandedLab="UtÃ¶kat"
_collapsedLab="Komprimerad"
_selectedLab="Markerad"

_expandNode="UtÃ¶ka noden %1"
_collapseNode="Komprimera noden %1"

_checkedPromptLab="Har angetts"
_nocheckedPromptLab="Har inte angetts"
_selectionPromptLab="vÃ¤rden Ã¤r lika med"
_noselectionPromptLab="inga vÃ¤rden"

_lovTextFieldLab="Skriv vÃ¤rden hÃ¤r"
_lovCalendarLab="Skriv datum hÃ¤r"
_lovPrevChunkLab="GÃ¥ till fÃ¶regÃ¥ende segment"
_lovNextChunkLab="GÃ¥ till nÃ¤sta segment"
_lovComboChunkLab="Segment"
_lovRefreshLab="Uppdatera"
_lovSearchFieldLab="Ange text att sÃ¶ka efter hÃ¤r"
_lovSearchLab="SÃ¶k"
_lovNormalLab="Normal"
_lovMatchCase="Matcha gemener/VERSALER"
_lovRefreshValuesLab="Uppdatera vÃ¤rden"

_calendarNextMonthLab="GÃ¥ till nÃ¤sta mÃ¥nad"
_calendarPrevMonthLab="GÃ¥ till fÃ¶regÃ¥ende mÃ¥nad"
_calendarNextYearLab="GÃ¥ till nÃ¤sta Ã¥r"
_calendarPrevYearLab="GÃ¥ till fÃ¶regÃ¥ende Ã¥r"
_calendarSelectionLab="Markerad dag"

_menuCheckLab="Markerat"
_menuDisableLab="Inaktiverad"
	
_level="NivÃ¥"
_closeTab="StÃ¤ng flik"
_of=" av"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="HjÃ¤lp"

_waitTitleLab="VÃ¤nta"
_cancelButtonLab="Avbryt"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Skift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Fler kantlinjer..."
_bordersTooltip=new Array
_bordersTooltip[0]="Ingen kantlinje"
_bordersTooltip[1]="VÃ¤nsterkant"
_bordersTooltip[2]="HÃ¶gerkant"
_bordersTooltip[3]="Nederkant"
_bordersTooltip[4]="Mellanbred nederkant"
_bordersTooltip[5]="Bred nederkant"
_bordersTooltip[6]="Ã–ver- och nederkant"
_bordersTooltip[7]="Ã–verkant och mellanbred nederkant"
_bordersTooltip[8]="Ã–verkant och bred nederkant"
_bordersTooltip[9]="Alla kantlinjer"
_bordersTooltip[10]="Alla mellanbreda kantlinjer"
_bordersTooltip[11]="Alla breda kantlinjer"