﻿/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />

//#region CswMobilePageOffline

function CswMobilePageOffline(offlineDef,$parent,mobileStorage) {
	/// <summary>
	///   Help Page class. Responsible for generating a Mobile help page.
	/// </summary>
    /// <param name="helpDef" type="Object">Help definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageOffline">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    if(isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }

    var $offline = $('<p>You must have internet connectivity to login.</p>');

    var p = {
	    level: -1,
	    DivId: 'sorrycharliediv',       // required
	    HeaderText: 'Sorry Charlie!',
	    theme: 'b',
	    $content: $offline
	};
	if (offlineDef) $.extend(p, offlineDef);

    var onlineValue = mobileStorage.onlineStatus();
    
    if( isNullOrEmpty(p.footerDef)) {
        p.footerDef = {
		    buttons: {
					online: { ID: p.DivId + '_gosyncstatus',
								text: onlineValue,
								cssClass: 'ui-btn-active onlineStatus ' + onlineValue.toLowerCase(),
								dataIcon: 'gear'
					},
		            refresh: { ID: p.DivId + '_refresh',
								text: 'Refresh',
								cssClass: 'refresh',
								dataIcon: 'refresh' 
		            },
	                fullsite: { ID: p.DivId + '_main',
	                    text: 'Full Site',
	                    href: 'Main.html',
	                    rel: 'external',
	                    dataIcon: 'home'
	                }
	        }
	    };
	}

	var offlinePage = new CswMobilePageFactory(p, $parent);
	var offlineHeader = offlinePage.mobileHeader;
	var offlineFooter = offlinePage.mobileFooter;
	var $content = offlinePage.$content;
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.mobileHeader = offlineHeader;
    this.mobileFooter = offlineFooter;
    this.$pageDiv = offlinePage.$pageDiv;

    this.onPageOpen = function() {
        this.$pageDiv.CswChangePage({ transition: 'slideup' });
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageOffline