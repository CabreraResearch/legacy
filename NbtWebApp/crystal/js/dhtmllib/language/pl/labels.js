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

_default="DomyÅ›lny"
_black="Czarny"
_brown="BrÄ…zowy"
_oliveGreen="Oliwkowozielony"
_darkGreen="Ciemnozielony"
_darkTeal="Ciemnozielonomodry"
_navyBlue="Granatowy"
_indigo="Indygo"
_darkGray="Ciemnoszary"
_darkRed="Ciemnoczerwony"
_orange="PomaraÅ„czowy"
_darkYellow="CiemnoÅ¼Ã³Å‚ty"
_green="Zielony"
_teal="Zielonomodry"
_blue="Niebieski"
_blueGray="Niebieskoszary"
_mediumGray="Åšrednioszary"
_red="Czerwony"
_lightOrange="JasnopomaraÅ„czowy"
_lime="Limonowy"
_seaGreen="Morska zieleÅ„"
_aqua="Akwamaryna"
_lightBlue="Jasnoniebieski"
_violet="Fioletowy"
_gray="Szary"
_magenta="Amarantowy"
_gold="ZÅ‚oty"
_yellow="Å»Ã³Å‚ty"
_brightGreen="Intensywny zielony"
_cyan="BÅ‚Ä™kitny"
_skyBlue="Lazurowy"
_plum="Åšliwkowy"
_lightGray="Jasnoszary"
_pink="RÃ³Å¼owy"
_tan="PastelowobrÄ…zowy"
_lightYellow="JasnoÅ¼Ã³Å‚ty"
_lightGreen="Jasnozielony"
_lightTurquoise="Jasnoturkusowy"
_paleBlue="Bladoniebieski"
_lavender="Lawendowy"
_white="BiaÅ‚y"
_lastUsed="Ostatnio uÅ¼ywany:"
_moreColors="WiÄ™cej kolorÃ³w..."

_month=new Array

_month[0]="StyczeÅ„"
_month[1]="Luty"
_month[2]="Marzec"
_month[3]="KwiecieÅ„"
_month[4]="Maj"
_month[5]="Czerwiec"
_month[6]="Lipiec"
_month[7]="SierpieÅ„"
_month[8]="WrzesieÅ„"
_month[9]="PaÅºdziernik"
_month[10]="Listopad"
_month[11]="GrudzieÅ„"

_day=new Array
_day[0]="Ni"
_day[1]="Pn"
_day[2]="Wt"
_day[3]="Åšr"
_day[4]="Cz"
_day[5]="Pt"
_day[6]="So"

_today="DziÅ›"

_AM="AM"
_PM="PM"

_closeDialog="Zamknij okno"

_lstMoveUpLab="PrzenieÅ› w gÃ³rÄ™"
_lstMoveDownLab="PrzenieÅ› w dÃ³Å‚"
_lstMoveLeftLab="PrzenieÅ› w lewo" 
_lstMoveRightLab="PrzenieÅ› w prawo"
_lstNewNodeLab="Dodaj filtr zagnieÅ¼dÅ¼ony"
_lstAndLabel="i"
_lstOrLabel="OR"
_lstSelectedLabel="Wybrano"
_lstQuickFilterLab="Dodaj szybki filtr"

_openMenu="Kliknij tutaj, aby uzyskaÄ‡ dostÄ™p do opcji {0}"
_openCalendarLab="OtwÃ³rz kalendarz"

_scroll_first_tab="PrzewiÅ„ do pierwszej karty"
_scroll_previous_tab="PrzewiÅ„ do poprzedniej karty"
_scroll_next_tab="PrzewiÅ„ do nastÄ™pnej karty"
_scroll_last_tab="PrzewiÅ„ do ostatniej karty"

_expandedLab="RozwiniÄ™ty"
_collapsedLab="ZwiniÄ™ty"
_selectedLab="Wybrano"

_expandNode="RozwiÅ„ wÄ™zeÅ‚ %1"
_collapseNode="ZwiÅ„ wÄ™zeÅ‚ %1"

_checkedPromptLab="Ustawiony"
_nocheckedPromptLab="Nieustawiony"
_selectionPromptLab="wartoÅ›ci rÃ³wne"
_noselectionPromptLab="bez wartoÅ›ci"

_lovTextFieldLab="Tutaj wpisz wartoÅ›ci"
_lovCalendarLab="Tutaj wpisz datÄ™"
_lovPrevChunkLab="PrzejdÅº do poprzedniej porcji danych"
_lovNextChunkLab="PrzejdÅº do nastÄ™pnej porcji danych"
_lovComboChunkLab="Porcja danych"
_lovRefreshLab="OdÅ›wieÅ¼"
_lovSearchFieldLab="Tutaj wpisz tekst do wyszukania"
_lovSearchLab="Wyszukaj"
_lovNormalLab="Normalna"
_lovMatchCase="UwzglÄ™dnij wielkoÅ›Ä‡ liter"
_lovRefreshValuesLab="OdÅ›wieÅ¼ wartoÅ›ci"

_calendarNextMonthLab="PrzejdÅº do nastÄ™pnego miesiÄ…ca"
_calendarPrevMonthLab="PrzejdÅº do poprzedniego miesiÄ…ca"
_calendarNextYearLab="PrzejdÅº do nastÄ™pnego roku"
_calendarPrevYearLab="PrzejdÅº do poprzedniego roku"
_calendarSelectionLab="Wybrany dzieÅ„"

_menuCheckLab="Zaznaczone"
_menuDisableLab="WyÅ‚Ä…czone"
	
_level="Poziom"
_closeTab="Zamknij kartÄ™"
_of=" z "

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="Pomoc"

_waitTitleLab="Czekaj"
_cancelButtonLab="Anuluj"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="WiÄ™cej krawÄ™dzi..."
_bordersTooltip=new Array
_bordersTooltip[0]="Bez obramowania"
_bordersTooltip[1]="Lewa krawÄ™dÅº"
_bordersTooltip[2]="Prawa krawÄ™dÅº"
_bordersTooltip[3]="KrawÄ™dÅº dolna"
_bordersTooltip[4]="Åšrednia dolna krawÄ™dÅº"
_bordersTooltip[5]="Gruba dolna krawÄ™dÅº"
_bordersTooltip[6]="GÃ³rna i dolna krawÄ™dÅº"
_bordersTooltip[7]="GÃ³rna i Å›rednia dolna krawÄ™dÅº"
_bordersTooltip[8]="GÃ³rna i gruba dolna krawÄ™dÅº"
_bordersTooltip[9]="Wszystkie krawÄ™dzie"
_bordersTooltip[10]="Wszystkie Å›rednie krawÄ™dzie"
_bordersTooltip[11]="Wszystkie grube krawÄ™dzie"