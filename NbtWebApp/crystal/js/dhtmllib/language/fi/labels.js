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

_default="Oletus"
_black="Musta"
_brown="Ruskea"
_oliveGreen="OliivinvihreÃ¤"
_darkGreen="TummanvihreÃ¤"
_darkTeal="Tumma sinivihreÃ¤"
_navyBlue="Laivastonsininen"
_indigo="Indigo"
_darkGray="Tummanharmaa"
_darkRed="Tummanpunainen"
_orange="Oranssi"
_darkYellow="Tummankeltainen"
_green="VihreÃ¤"
_teal="SinivihreÃ¤"
_blue="Sininen"
_blueGray="Siniharmaa"
_mediumGray="Keskiharmaa"
_red="Punainen"
_lightOrange="Vaaleanoranssi"
_lime="LimenvihreÃ¤"
_seaGreen="MerenvihreÃ¤"
_aqua="Vedensininen"
_lightBlue="Vaaleansininen"
_violet="Violetti"
_gray="Harmaa"
_magenta="Purppuranpunainen"
_gold="Kulta"
_yellow="Keltainen"
_brightGreen="KirkkaanvihreÃ¤"
_cyan="Turkoosi"
_skyBlue="Taivaansininen"
_plum="Luumu"
_lightGray="Vaaleanharmaa"
_pink="Vaaleanpunainen"
_tan="Kellanruskea"
_lightYellow="Vaaleankeltainen"
_lightGreen="VaaleanvihreÃ¤"
_lightTurquoise="Vaalea turkoosi"
_paleBlue="Haalean sininen"
_lavender="Laventelinsininen"
_white="Valkoinen"
_lastUsed="Viimeksi kÃ¤ytetty:"
_moreColors="LisÃ¤Ã¤ vÃ¤rejÃ¤..."

_month=new Array

_month[0]="TAMMIKUU"
_month[1]="HELMIKUU"
_month[2]="MAALISKUU"
_month[3]="HUHTIKUU"
_month[4]="TOUKOKUU"
_month[5]="KESÃ„KUU"
_month[6]="HEINÃ„KUU"
_month[7]="ELOKUU"
_month[8]="SYYSKUU"
_month[9]="LOKAKUU"
_month[10]="MARRASKUU"
_month[11]="JOULUKUU"

_day=new Array
_day[0]="S"
_day[1]="M"
_day[2]="T"
_day[3]="K"
_day[4]="T"
_day[5]="P"
_day[6]="L"

_today="TÃ¤nÃ¤Ã¤n"

_AM="ap"
_PM="ip"

_closeDialog="Sulje ikkuna"

_lstMoveUpLab="SiirrÃ¤ ylÃ¶s"
_lstMoveDownLab="SiirrÃ¤ alas"
_lstMoveLeftLab="SiirrÃ¤ vasemmalle" 
_lstMoveRightLab="SiirrÃ¤ oikealle"
_lstNewNodeLab="LisÃ¤Ã¤ sisÃ¤kkÃ¤inen suodatin"
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="Valittu"
_lstQuickFilterLab="LisÃ¤Ã¤ pikasuodatin"

_openMenu="KÃ¤ytÃ¤ asetuksia {0} napsauttamalla tÃ¤tÃ¤"
_openCalendarLab="Avaa kalenteri"

_scroll_first_tab="VieritÃ¤ ensimmÃ¤iseen vÃ¤lilehteen"
_scroll_previous_tab="VieritÃ¤ edelliseen vÃ¤lilehteen"
_scroll_next_tab="VieritÃ¤ seuraavaan vÃ¤lilehteen"
_scroll_last_tab="VieritÃ¤ viimeiseen vÃ¤lilehteen"

_expandedLab="Laajennettu"
_collapsedLab="Tiivistetty"
_selectedLab="Valittu"

_expandNode="Laajenna solmu %1"
_collapseNode="TiivistÃ¤ solmu %1"

_checkedPromptLab="MÃ¤Ã¤ritetty"
_nocheckedPromptLab="Ei mÃ¤Ã¤ritetty"
_selectionPromptLab="arvot ovat yhtÃ¤ kuin"
_noselectionPromptLab="Ei arvoja"

_lovTextFieldLab="Kirjoita arvot tÃ¤hÃ¤n"
_lovCalendarLab="Kirjoita pÃ¤ivÃ¤mÃ¤Ã¤rÃ¤ tÃ¤hÃ¤n"
_lovPrevChunkLab="Siirry edelliseen erÃ¤Ã¤n"
_lovNextChunkLab="Siirry seuraavaan erÃ¤Ã¤n"
_lovComboChunkLab="ErÃ¤"
_lovRefreshLab="PÃ¤ivitÃ¤"
_lovSearchFieldLab="Kirjoita hakuteksti tÃ¤hÃ¤n"
_lovSearchLab="Haku"
_lovNormalLab="Normaali"
_lovMatchCase="Sama kirjainkoko"
_lovRefreshValuesLab="PÃ¤ivitÃ¤ arvot"

_calendarNextMonthLab="Siirry seuraavaan kuukauteen"
_calendarPrevMonthLab="Siirry edelliseen kuukauteen"
_calendarNextYearLab="Siirry seuraavaan vuoteen"
_calendarPrevYearLab="Siirry edelliseen vuoteen"
_calendarSelectionLab="Valittu pÃ¤ivÃ¤"

_menuCheckLab="Valittu"
_menuDisableLab="Poistettu kÃ¤ytÃ¶stÃ¤"
	
_level="Taso"
_closeTab="Sulje vÃ¤lilehti"
_of=" / "

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="Ohje"

_waitTitleLab="Odota"
_cancelButtonLab="Peruuta"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="LisÃ¤Ã¤ reunoja..."
_bordersTooltip=new Array
_bordersTooltip[0]="Ei reunaa"
_bordersTooltip[1]="Vasen reuna"
_bordersTooltip[2]="Oikea reuna"
_bordersTooltip[3]="Alareuna"
_bordersTooltip[4]="Keskikokoinen alareuna"
_bordersTooltip[5]="Lihavoitu alareuna"
_bordersTooltip[6]="YlÃ¤- ja alareuna"
_bordersTooltip[7]="YlÃ¤reuna ja keskikokoinen alareuna"
_bordersTooltip[8]="YlÃ¤reuna ja lihavoitu alareuna"
_bordersTooltip[9]="Kaikki reunat"
_bordersTooltip[10]="Kaikki keskikokoiset reunat"
_bordersTooltip[11]="Kaikki lihavoidut reunat"