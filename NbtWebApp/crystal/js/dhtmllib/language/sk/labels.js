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

_default="PredvolenÃ©"
_black="ÄŒierna"
_brown="HnedÃ¡"
_oliveGreen="OlivovozelenÃ¡"
_darkGreen="TmavozelenÃ¡"
_darkTeal="TmavosivozelenÃ¡"
_navyBlue="NÃ¡mornÃ­cka modrÃ¡"
_indigo="IndigovÃ¡"
_darkGray="TmavosivÃ¡"
_darkRed="TmavoÄervenÃ¡"
_orange="OranÅ¾ovÃ¡"
_darkYellow="TmavoÅ¾ltÃ¡"
_green="ZelenÃ¡"
_teal="SivozelenÃ¡"
_blue="ModrÃ¡"
_blueGray="ModrosivÃ¡"
_mediumGray="Stredne sivÃ¡"
_red="ÄŒervenÃ¡"
_lightOrange="SvetlooranÅ¾ovÃ¡"
_lime="Å½ltozelenÃ¡"
_seaGreen="MorskÃ¡ zelenÃ¡"
_aqua="AkvamarÃ­novÃ¡"
_lightBlue="JasnomodrÃ¡"
_violet="FialovÃ¡"
_gray="SivÃ¡"
_magenta="PurpurovÃ¡"
_gold="ZlatÃ¡"
_yellow="Å½ltÃ¡"
_brightGreen="JasnozelenÃ¡"
_cyan="AzÃºrovÃ¡"
_skyBlue="NebeskÃ¡ modrÃ¡"
_plum="SlivkovÃ¡"
_lightGray="SvetlosivÃ¡"
_pink="RuÅ¾ovÃ¡"
_tan="Å½ltohnedÃ¡"
_lightYellow="SvetloÅ¾ltÃ¡"
_lightGreen="SvetlozelenÃ¡"
_lightTurquoise="SvetlotyrkysovÃ¡"
_paleBlue="BledomodrÃ¡"
_lavender="LevanduÄ¾ovÃ¡"
_white="Biela"
_lastUsed="Naposledy pouÅ¾itÃ¡:"
_moreColors="ÄŽalÅ¡ie farby..."

_month=new Array

_month[0]="JANUÃR"
_month[1]="FEBRUÃR"
_month[2]="MAREC"
_month[3]="APRÃL"
_month[4]="MÃJ"
_month[5]="JÃšN"
_month[6]="JÃšL"
_month[7]="AUGUST"
_month[8]="SEPTEMBER"
_month[9]="OKTÃ“BER"
_month[10]="NOVEMBER"
_month[11]="DECEMBER"

_day=new Array
_day[0]="N"
_day[1]="P"
_day[2]="U"
_day[3]="S"
_day[4]="Å "
_day[5]="P"
_day[6]="S"

_today="ï»¿Dnes"

_AM="dopoludnia"
_PM="odpoludnia"

_closeDialog="ZavrieÅ¥ okno"

_lstMoveUpLab="PremiestniÅ¥ nahor"
_lstMoveDownLab="PremiestniÅ¥ nadol"
_lstMoveLeftLab="PremiestniÅ¥ doÄ¾ava" 
_lstMoveRightLab="PremiestniÅ¥ doprava"
_lstNewNodeLab="PridaÅ¥ vnorenÃ½ filter"
_lstAndLabel="A"
_lstOrLabel="ALEBO"
_lstSelectedLabel="VybratÃ©"
_lstQuickFilterLab="PridaÅ¥ rÃ½chly filter"

_openMenu="Kliknite sem pre prÃ­stup k moÅ¾nostiam {0}"
_openCalendarLab="OtvoriÅ¥ kalendÃ¡r"

_scroll_first_tab="PosunÃºÅ¥ na prvÃº kartu"
_scroll_previous_tab="PosunÃºÅ¥ na predchÃ¡dzajÃºcu kartu"
_scroll_next_tab="PosunÃºÅ¥ na ÄalÅ¡iu kartu"
_scroll_last_tab="PosunÃºÅ¥ na poslednÃº kartu"

_expandedLab="RozbalenÃ©"
_collapsedLab="ZbalenÃ©"
_selectedLab="VybratÃ©"

_expandNode="RozbaliÅ¥ uzol %1"
_collapseNode="ZbaliÅ¥ uzol %1"

_checkedPromptLab="NastavenÃ©"
_nocheckedPromptLab="NenastavenÃ©"
_selectionPromptLab="hodnoty rovnajÃºce sa"
_noselectionPromptLab="bez hodnÃ´t"

_lovTextFieldLab="Zadajte hodnoty sem"
_lovCalendarLab="Zadajte dÃ¡tum sem"
_lovPrevChunkLab="PrejsÅ¥ na predchÃ¡dzajÃºci blok"
_lovNextChunkLab="PrejsÅ¥ na nasledujÃºci blok"
_lovComboChunkLab="Blok"
_lovRefreshLab="ObnoviÅ¥"
_lovSearchFieldLab="Sem zadajte text na vyhÄ¾adanie"
_lovSearchLab="HÄ¾adaÅ¥"
_lovNormalLab="NormÃ¡lne"
_lovMatchCase="RozliÅ¡ovaÅ¥ malÃ© a VEÄ½KÃ‰ PÃSMENÃ"
_lovRefreshValuesLab="ObnoviÅ¥ hodnoty"

_calendarNextMonthLab="PrejsÅ¥ na nasledujÃºci mesiac"
_calendarPrevMonthLab="PrejsÅ¥ na predchÃ¡dzajÃºci mesiac"
_calendarNextYearLab="PrejsÅ¥ na nasledujÃºci rok"
_calendarPrevYearLab="PrejsÅ¥ na predchÃ¡dzajÃºci rok"
_calendarSelectionLab="VybranÃ½ deÅˆ"

_menuCheckLab="ZaÄiarknutÃ©"
_menuDisableLab="VypnutÃ©"
	
_level="ÃšroveÅˆ"
_closeTab="ZavrieÅ¥ Kartu"
_of="z"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="PomocnÃ­k"

_waitTitleLab="ÄŒakajte, prosÃ­m"
_cancelButtonLab="ZruÅ¡iÅ¥"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt +"

_bordersMoreColorsLabel="ÄŽalÅ¡ie orÃ¡movania..."
_bordersTooltip=new Array
_bordersTooltip[0]="Bez orÃ¡movania"
_bordersTooltip[1]="Ä½avÃ© orÃ¡movanie"
_bordersTooltip[2]="PravÃ© orÃ¡movanie"
_bordersTooltip[3]="SpodnÃ© orÃ¡movanie"
_bordersTooltip[4]="Stredne silnÃ© spodnÃ© orÃ¡movanie"
_bordersTooltip[5]="SilnÃ© spodnÃ© orÃ¡movanie"
_bordersTooltip[6]="HornÃ© a spodnÃ© orÃ¡movanie"
_bordersTooltip[7]="HornÃ© a stredne silnÃ© spodnÃ© orÃ¡movanie"
_bordersTooltip[8]="HornÃ© a silnÃ© spodnÃ© orÃ¡movanie"
_bordersTooltip[9]="VÅ¡etky orÃ¡movania"
_bordersTooltip[10]="VÅ¡etky stredne silnÃ© orÃ¡movania"
_bordersTooltip[11]="VÅ¡etky silnÃ© orÃ¡movania"