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
_oliveGreen="OlivengrÃ¸nn"
_darkGreen="MÃ¸rkegrÃ¸nn"
_darkTeal="MÃ¸rk blÃ¥grÃ¸nn"
_navyBlue="MarineblÃ¥"
_indigo="Indigo"
_darkGray="MÃ¸rk grÃ¥"
_darkRed="MÃ¸rkerÃ¸d"
_orange="Oransje"
_darkYellow="MÃ¸rkegul"
_green="GrÃ¸nn"
_teal="BlÃ¥grÃ¸nn"
_blue="BlÃ¥"
_blueGray="BlÃ¥grÃ¥"
_mediumGray="MellomgrÃ¥"
_red="RÃ¸d"
_lightOrange="Lys oransje"
_lime="SitrusgrÃ¸nn"
_seaGreen="SjÃ¸grÃ¸nn"
_aqua="Akvamarin"
_lightBlue="LyseblÃ¥"
_violet="Fiolett"
_gray="GrÃ¥"
_magenta="Magenta"
_gold="Gull"
_yellow="Gul"
_brightGreen="Skarp grÃ¸nn"
_cyan="Cyan"
_skyBlue="HimmelblÃ¥"
_plum="Plomme"
_lightGray="Lys grÃ¥"
_pink="Rosa"
_tan="Lysebrun"
_lightYellow="Lys gul"
_lightGreen="LysegrÃ¸nn"
_lightTurquoise="Lys turkis"
_paleBlue="BlekblÃ¥"
_lavender="Lavendel"
_white="Hvit"
_lastUsed="Sist brukte:"
_moreColors="Flere farger..."

_month=new Array

_month[0]="JANUAR"
_month[1]="FEBRUAR"
_month[2]="MARS"
_month[3]="APRIL"
_month[4]="MAI"
_month[5]="JUNI"
_month[6]="JULI"
_month[7]="AUGUST"
_month[8]="SEPTEMBER"
_month[9]="OKTOBER"
_month[10]="NOVEMBER"
_month[11]="DESEMBER"

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

_closeDialog="Lukk vindu"

_lstMoveUpLab="Flytt opp"
_lstMoveDownLab="Flytt ned"
_lstMoveLeftLab="Flytt  mot venstre" 
_lstMoveRightLab="Flytt mot hÃ¸yre"
_lstNewNodeLab="Legg til nÃ¸stet filter"
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="Valgt"
_lstQuickFilterLab="Legg til hurtigfilter"

_openMenu="Klikk her for Ã¥ fÃ¥ tilgang til alternativer for {0}"
_openCalendarLab="Ã…pne kalender"

_scroll_first_tab="Rull til fÃ¸rste kategori"
_scroll_previous_tab="Rull til forrige kategori"
_scroll_next_tab="Rull til neste kategori"
_scroll_last_tab="Rull til siste kategori"

_expandedLab="Utvidet"
_collapsedLab="SammenslÃ¥tt"
_selectedLab="Valgt"

_expandNode="Utvid knutepunkt %1"
_collapseNode="SlÃ¥ sammen knutepunkt %1"

_checkedPromptLab="Angitt"
_nocheckedPromptLab="Ikke angitt"
_selectionPromptLab="verdier lik"
_noselectionPromptLab="ingen verdier"

_lovTextFieldLab="Skriv inn verdier her"
_lovCalendarLab="Skriv inn data her"
_lovPrevChunkLab="GÃ¥ til forrige skive"
_lovNextChunkLab="GÃ¥ til neste skive"
_lovComboChunkLab="Skive"
_lovRefreshLab="Oppdater"
_lovSearchFieldLab="Skriv inn tekst for sÃ¸k her"
_lovSearchLab="SÃ¸k"
_lovNormalLab="Normal"
_lovMatchCase="Skill mellom store og smÃ¥ bokstaver"
_lovRefreshValuesLab="Oppdater verdier"

_calendarNextMonthLab="GÃ¥ til neste mÃ¥ned"
_calendarPrevMonthLab="GÃ¥ til forrige mÃ¥ned"
_calendarNextYearLab="GÃ¥ til neste Ã¥r"
_calendarPrevYearLab="GÃ¥ til forrige Ã¥r"
_calendarSelectionLab="Valgt dag "

_menuCheckLab="Merket"
_menuDisableLab="Deaktivert"
	
_level="NivÃ¥"
_closeTab="Lukk kategori"
_of=" av "

_RGBTxtBegin= "RGB ("
_RGBTxtEnd= ")"

_helpLab="Hjelp"

_waitTitleLab="Vent litt"
_cancelButtonLab="Avbryt"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Flere kantlinjer..."
_bordersTooltip=new Array
_bordersTooltip[0]="Ingen kantlinje"
_bordersTooltip[1]="Venstre kantlinje"
_bordersTooltip[2]="HÃ¸yre kantlinje"
_bordersTooltip[3]="Nedre kantlinje"
_bordersTooltip[4]="Middels nedre kantlinje"
_bordersTooltip[5]="Bred nedre kantlinje"
_bordersTooltip[6]="Ã˜vre og nedre kantlinje"
_bordersTooltip[7]="Ã˜vre og middels nedre kantlinje"
_bordersTooltip[8]="Ã˜vre og bred nedre kantlinje"
_bordersTooltip[9]="Alle kantlinjer"
_bordersTooltip[10]="Alle kantlinjer middels"
_bordersTooltip[11]="Alle kantlinjer brede"