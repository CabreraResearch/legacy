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

_default="AlapÃ©rtelmezett"
_black="Fekete"
_brown="Barna"
_oliveGreen="OlajzÃ¶ld"
_darkGreen="SÃ¶tÃ©tzÃ¶ld"
_darkTeal="SÃ¶tÃ©t pÃ¡vakÃ©k"
_navyBlue="TengerÃ©szkÃ©k"
_indigo="IndigÃ³kÃ©k"
_darkGray="SÃ¶tÃ©tszÃ¼rke"
_darkRed="SÃ¶tÃ©tpiros"
_orange="NarancssÃ¡rga"
_darkYellow="OkkersÃ¡rga"
_green="ZÃ¶ld"
_teal="SÃ¶tÃ©tciÃ¡n"
_blue="KÃ©k"
_blueGray="KÃ©kesszÃ¼rke"
_mediumGray="KÃ¶zÃ©pszÃ¼rke"
_red="Piros"
_lightOrange="VilÃ¡gosnarancs"
_lime="CitromzÃ¶ld"
_seaGreen="TengerzÃ¶ld"
_aqua="TÃ¼rkiz"
_lightBlue="VilÃ¡goskÃ©k"
_violet="IbolyakÃ©k"
_gray="SzÃ¼rke"
_magenta="Magenta"
_gold="AranysÃ¡rga"
_yellow="SÃ¡rga"
_brightGreen="Ã‰lÃ©nkzÃ¶ld"
_cyan="CiÃ¡nkÃ©k"
_skyBlue="Ã‰gszÃ­nkÃ©k"
_plum="SzilvakÃ©k"
_lightGray="VilÃ¡gosszÃ¼rke"
_pink="RÃ³zsaszÃ­n"
_tan="SÃ¡rgÃ¡sbarna"
_lightYellow="VilÃ¡gossÃ¡rga"
_lightGreen="VilÃ¡goszÃ¶ld"
_lightTurquoise="VilÃ¡gostÃ¼rkiz"
_paleBlue="HalvÃ¡nykÃ©k"
_lavender="Levendula"
_white="FehÃ©r"
_lastUsed="LegutÃ³bb hasznÃ¡lt:"
_moreColors="TovÃ¡bbi szÃ­nek..."

_month=new Array

_month[0]="JANUÃR"
_month[1]="FEBRUÃR"
_month[2]="MÃRCIUS"
_month[3]="ÃPRILIS"
_month[4]="MÃJUS"
_month[5]="JÃšNIUS"
_month[6]="JÃšLIUS"
_month[7]="AUGUSZTUS"
_month[8]="SZEPTEMBER"
_month[9]="OKTÃ“BER"
_month[10]="NOVEMBER"
_month[11]="DECEMBER"

_day=new Array
_day[0]="V"
_day[1]="H"
_day[2]="K"
_day[3]="S"
_day[4]="C"
_day[5]="P"
_day[6]="S"

_today="Ma"

_AM="de."
_PM="du."

_closeDialog="Ablak bezÃ¡rÃ¡sa"

_lstMoveUpLab="FelfelÃ©"
_lstMoveDownLab="LefelÃ©"
_lstMoveLeftLab="Balra" 
_lstMoveRightLab="Jobbra"
_lstNewNodeLab="BeÃ¡gyazott szÅ±rÅ‘ beÃ¡llÃ­tÃ¡sa"
_lstAndLabel="Ã‰S"
_lstOrLabel="VAGY"
_lstSelectedLabel="KijelÃ¶lve"
_lstQuickFilterLab="GyorsszÅ±rÅ‘ beÃ¡llÃ­tÃ¡sa"

_openMenu="{0} beÃ¡llÃ­tÃ¡sainak megnyitÃ¡sa"
_openCalendarLab="NaptÃ¡r megnyitÃ¡sa"

_scroll_first_tab="GÃ¶rgetÃ©s az elsÅ‘ lapra"
_scroll_previous_tab="GÃ¶rgetÃ©s az elÅ‘zÅ‘ lapra"
_scroll_next_tab="GÃ¶rgetÃ©s a kÃ¶vetkezÅ‘ lapra"
_scroll_last_tab="GÃ¶rgetÃ©s az utolsÃ³ lapra"

_expandedLab="Kibontva"
_collapsedLab="VisszazÃ¡rva"
_selectedLab="KijelÃ¶lve"

_expandNode="%1 csomÃ³pont kibontÃ¡sa"
_collapseNode="%1 csomÃ³pont visszazÃ¡rÃ¡sa"

_checkedPromptLab="BeÃ¡llÃ­tva"
_nocheckedPromptLab="Nincs beÃ¡llÃ­tva"
_selectionPromptLab="ezzel egyenlÅ‘ Ã©rtÃ©kek"
_noselectionPromptLab="nincs Ã©rtÃ©k"

_lovTextFieldLab="Ãrja be az Ã©rtÃ©keket."
_lovCalendarLab="Ãrja be a dÃ¡tumot."
_lovPrevChunkLab="UgrÃ¡s az elÅ‘zÅ‘ rÃ©szre"
_lovNextChunkLab="UgrÃ¡s a kÃ¶vetkezÅ‘ rÃ©szre"
_lovComboChunkLab="RÃ©sz"
_lovRefreshLab="FrissÃ­tÃ©s"
_lovSearchFieldLab="Ãrja be a keresett szÃ¶veget."
_lovSearchLab="KeresÃ©s"
_lovNormalLab="NormÃ¡l"
_lovMatchCase="Kis- Ã©s nagybetÅ±k megkÃ¼lÃ¶nbÃ¶ztetÃ©se"
_lovRefreshValuesLab="Ã‰rtÃ©kek frissÃ­tÃ©se"

_calendarNextMonthLab="KÃ¶vetkezÅ‘ hÃ³napra"
_calendarPrevMonthLab="ElÅ‘zÅ‘ hÃ³napra"
_calendarNextYearLab="KÃ¶vetkezÅ‘ Ã©vre"
_calendarPrevYearLab="ElÅ‘zÅ‘ Ã©vre"
_calendarSelectionLab="KijelÃ¶lt nap "

_menuCheckLab="BejelÃ¶lve"
_menuDisableLab="Letiltva"
	
_level="Szint"
_closeTab="Lap bezÃ¡rÃ¡sa"
_of=" / "

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="SÃºgÃ³"

_waitTitleLab="VÃ¡rjon"
_cancelButtonLab="MÃ©gse"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="TovÃ¡bbi szegÃ©lyek..."
_bordersTooltip=new Array
_bordersTooltip[0]="Nincs szegÃ©ly"
_bordersTooltip[1]="Bal szegÃ©ly"
_bordersTooltip[2]="Jobb szegÃ©ly"
_bordersTooltip[3]="AlsÃ³ szegÃ©ly"
_bordersTooltip[4]="KÃ¶zÃ©pvastag alsÃ³ szegÃ©ly"
_bordersTooltip[5]="Vastag alsÃ³ szegÃ©ly"
_bordersTooltip[6]="FelsÅ‘ Ã©s alsÃ³ szegÃ©ly"
_bordersTooltip[7]="FelsÅ‘ Ã©s kÃ¶zÃ©pvastag alsÃ³ szegÃ©ly"
_bordersTooltip[8]="FelsÅ‘ Ã©s vastag alsÃ³ szegÃ©ly"
_bordersTooltip[9]="SzegÃ©ly mindenhol"
_bordersTooltip[10]="KÃ¶zÃ©pvastag szegÃ©ly mindenhol"
_bordersTooltip[11]="Vastag szegÃ©ly mindenhol"