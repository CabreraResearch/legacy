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

_default="Par dÃ©faut"
_black="Noir"
_brown="Marron"
_oliveGreen="Vert olive"
_darkGreen="Vert foncÃ©"
_darkTeal="Bleu-vert foncÃ©"
_navyBlue="Bleu marine"
_indigo="Indigo"
_darkGray="Gris foncÃ©"
_darkRed="Rouge foncÃ©"
_orange="Orange"
_darkYellow="Jaune foncÃ©"
_green="Vert"
_teal="Bleu-vert"
_blue="Bleu"
_blueGray="Bleu-gris"
_mediumGray="Gris moyen"
_red="Rouge"
_lightOrange="Orange clair"
_lime="Citron vert"
_seaGreen="Vert marin"
_aqua="Vert d\'eau"
_lightBlue="Bleu clair"
_violet="Violet"
_gray="Gris"
_magenta="Magenta"
_gold="Or"
_yellow="Jaune"
_brightGreen="Vert brillant"
_cyan="Cyan"
_skyBlue="Bleu ciel"
_plum="Prune"
_lightGray="Gris clair"
_pink="Rose"
_tan="Ocre"
_lightYellow="Jaune clair"
_lightGreen="Vert clair"
_lightTurquoise="Turquoise clair"
_paleBlue="Bleu pÃ¢le"
_lavender="Lavande"
_white="Blanc"
_lastUsed="DerniÃ¨re couleur utilisÃ©e :"
_moreColors="Autres couleurs..."

_month=new Array

_month[0]="JANVIER"
_month[1]="FEVRIER"
_month[2]="MARS"
_month[3]="AVRIL"
_month[4]="MAI"
_month[5]="JUIN"
_month[6]="JUILLET"
_month[7]="AOUT"
_month[8]="SEPTEMBRE"
_month[9]="OCTOBRE"
_month[10]="NOVEMBRE"
_month[11]="DECEMBRE"

_day=new Array
_day[0]="D"
_day[1]="L"
_day[2]="Ma"
_day[3]="Mer"
_day[4]="J"
_day[5]="V"
_day[6]="S"

_today="Aujourd\'hui"

_AM="a.m."
_PM="p.m."

_closeDialog="Fermer la fenÃªtre"

_lstMoveUpLab="Monter"
_lstMoveDownLab="Descendre"
_lstMoveLeftLab="DÃ©placer Ã  gauche" 
_lstMoveRightLab="DÃ©placer Ã  droite"
_lstNewNodeLab="Ajouter un filtre imbriquÃ©"
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="SÃ©lectionnÃ©"
_lstQuickFilterLab="Ajouter un filtre express"

_openMenu="Cliquez ici pour accÃ©der aux options {0}"
_openCalendarLab="Ouvrir le calendrier"

_scroll_first_tab="Faire dÃ©filer jusqu\'au premier onglet"
_scroll_previous_tab="Faire dÃ©filer jusqu\'Ã  l\'onglet prÃ©cÃ©dent"
_scroll_next_tab="Faire dÃ©filer jusqu\'Ã  l\'onglet suivant"
_scroll_last_tab="Faire dÃ©filer jusqu\'au dernier onglet"

_expandedLab="DÃ©veloppÃ©"
_collapsedLab="RÃ©duit"
_selectedLab="SÃ©lectionnÃ©"

_expandNode="DÃ©velopper le nÅ“ud %1"
_collapseNode="RÃ©duire le nÅ“ud %1"

_checkedPromptLab="DÃ©fini"
_nocheckedPromptLab="Non dÃ©fini"
_selectionPromptLab="valeurs Ã©gales Ã "
_noselectionPromptLab="aucune valeur"

_lovTextFieldLab="Entrez des valeurs ici"
_lovCalendarLab="Entrez la date ici"
_lovPrevChunkLab="Aller au bloc prÃ©cÃ©dent"
_lovNextChunkLab="Aller au bloc suivant"
_lovComboChunkLab="Bloc"
_lovRefreshLab="Actualiser"
_lovSearchFieldLab="Saisissez ici le texte Ã  rechercher"
_lovSearchLab="Rechercher"
_lovNormalLab="Normal"
_lovMatchCase="Respecter la casse"
_lovRefreshValuesLab="Actualiser les valeurs"

_calendarNextMonthLab="Aller au mois suivant"
_calendarPrevMonthLab="Aller au mois prÃ©cÃ©dent"
_calendarNextYearLab="Aller Ã  l\'annÃ©e suivante"
_calendarPrevYearLab="Aller Ã  l\'annÃ©e prÃ©cÃ©dente"
_calendarSelectionLab="Jour sÃ©lectionnÃ© "

_menuCheckLab="ActivÃ©"
_menuDisableLab="DÃ©sactivÃ©"
	
_level="Niveau"
_closeTab="Fermer l\'onglet"
_of=" sur"

_RGBTxtBegin= "RVB("
_RGBTxtEnd= ")"

_helpLab="Aide"

_waitTitleLab="Veuillez patienter"
_cancelButtonLab="Annuler"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Maj+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Autres bordures..."
_bordersTooltip=new Array
_bordersTooltip[0]="Aucune bordure"
_bordersTooltip[1]="Bordure de gauche"
_bordersTooltip[2]="Bordure de droite"
_bordersTooltip[3]="Bordure infÃ©rieure"
_bordersTooltip[4]="Bordure infÃ©rieure moyenne"
_bordersTooltip[5]="Bordure infÃ©rieure Ã©paisse"
_bordersTooltip[6]="Bordure supÃ©rieure et infÃ©rieure"
_bordersTooltip[7]="Bordure supÃ©rieure et infÃ©rieure moyenne"
_bordersTooltip[8]="Bordure supÃ©rieure et infÃ©rieure Ã©paisse"
_bordersTooltip[9]="Toutes les bordures"
_bordersTooltip[10]="Toutes les bordures moyennes"
_bordersTooltip[11]="Toutes les bordures Ã©paisses"