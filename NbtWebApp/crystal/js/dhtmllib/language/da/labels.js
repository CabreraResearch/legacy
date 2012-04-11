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
_black="Sort"
_brown="Brun"
_oliveGreen="OlivengrÃ¸n"
_darkGreen="MÃ¸rkegrÃ¸n"
_darkTeal="DybblÃ¥"
_navyBlue="MarineblÃ¥"
_indigo="Indigo"
_darkGray="MÃ¸rkegrÃ¥"
_darkRed="MÃ¸rkerÃ¸d"
_orange="Orange"
_darkYellow="MÃ¸rkegul"
_green="GrÃ¸n"
_teal="BlÃ¥grÃ¸n"
_blue="BlÃ¥"
_blueGray="BlÃ¥grÃ¥"
_mediumGray="MellemgrÃ¥"
_red="RÃ¸d"
_lightOrange="Lys orange"
_lime="Lime"
_seaGreen="HavgrÃ¸n"
_aqua="Akvamarin"
_lightBlue="LyseblÃ¥"
_violet="Violet"
_gray="GrÃ¥"
_magenta="Magenta"
_gold="Guld"
_yellow="Gul"
_brightGreen="KnaldgrÃ¸n"
_cyan="Cyan"
_skyBlue="HimmelblÃ¥"
_plum="Blomme"
_lightGray="LysegrÃ¥"
_pink="Pink"
_tan="Tan"
_lightYellow="Lysegul"
_lightGreen="LysegrÃ¸n"
_lightTurquoise="Blegturkis"
_paleBlue="BlegblÃ¥"
_lavender="Lavendel"
_white="Hvid"
_lastUsed="Sidst anvendt:"
_moreColors="Flere farver..."

_month=new Array

_month[0]="JANUAR"
_month[1]="FEBRUAR"
_month[2]="MARTS"
_month[3]="APRIL"
_month[4]="MAJ"
_month[5]="JUNI"
_month[6]="JULI"
_month[7]="AUGUST"
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

_AM="AM"
_PM="PM"

_closeDialog="Luk vindue"

_lstMoveUpLab="Flyt op"
_lstMoveDownLab="Flyt ned"
_lstMoveLeftLab="Flyt til venstre" 
_lstMoveRightLab="Flyt til hÃ¸jre"
_lstNewNodeLab="TilfÃ¸j indlejret filter"
_lstAndLabel="OG"
_lstOrLabel="ELLER"
_lstSelectedLabel="Valgt"
_lstQuickFilterLab="TilfÃ¸j hurtigfilter"

_openMenu="Klik her for at Ã¥bne {0}-indstillinger"
_openCalendarLab="Ã…bn kalender"

_scroll_first_tab="Rul til fÃ¸rste fane"
_scroll_previous_tab="Rul til forrige fane"
_scroll_next_tab="Rul til nÃ¦ste fane"
_scroll_last_tab="Rul til sidste fane"

_expandedLab="Udvidet"
_collapsedLab="Skjult"
_selectedLab="Valgt"

_expandNode="Udvid node %1"
_collapseNode="Skjul node %1"

_checkedPromptLab="Angivet"
_nocheckedPromptLab="Ikke angivet"
_selectionPromptLab="vÃ¦rdier er lig med"
_noselectionPromptLab="ingen vÃ¦rdier"

_lovTextFieldLab="Indtast vÃ¦rdier her"
_lovCalendarLab="Indtast dato her"
_lovPrevChunkLab="GÃ¥ til forrige segment"
_lovNextChunkLab="GÃ¥ til nÃ¦ste segment"
_lovComboChunkLab="Segment"
_lovRefreshLab="Opdater"
_lovSearchFieldLab="Indtast tekst, der sÃ¸ges efter, her"
_lovSearchLab="SÃ¸g"
_lovNormalLab="Normal"
_lovMatchCase="Forskel pÃ¥ store og smÃ¥ bogstaver"
_lovRefreshValuesLab="Opdater vÃ¦rdier"

_calendarNextMonthLab="GÃ¥ til nÃ¦ste mÃ¥ned"
_calendarPrevMonthLab="GÃ¥ til forrige mÃ¥ned"
_calendarNextYearLab="GÃ¥ til nÃ¦ste Ã¥r"
_calendarPrevYearLab="GÃ¥ til forrige Ã¥r"
_calendarSelectionLab="Valgt dag "

_menuCheckLab="Markeret"
_menuDisableLab="Deaktiveret"
	
_level="Niveau"
_closeTab="Luk fane"
_of="af"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="HjÃ¦lp"

_waitTitleLab="Vent venligst"
_cancelButtonLab="Annuller"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Flere rammer..."
_bordersTooltip=new Array
_bordersTooltip[0]="Ingen ramme"
_bordersTooltip[1]="Venstre ramme"
_bordersTooltip[2]="HÃ¸jre ramme"
_bordersTooltip[3]="Nederste ramme"
_bordersTooltip[4]="Medium nederste ramme"
_bordersTooltip[5]="Tyk nederste ramme"
_bordersTooltip[6]="Ã˜verste og nederste ramme"
_bordersTooltip[7]="Ã˜verste og medium nederste ramme"
_bordersTooltip[8]="Ã˜verste og tyk nederste ramme"
_bordersTooltip[9]="Alle rammer"
_bordersTooltip[10]="Alle rammer medium"
_bordersTooltip[11]="Alle rammer tykke"