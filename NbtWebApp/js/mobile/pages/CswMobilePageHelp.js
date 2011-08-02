/// <reference path="../../_Global.js" />
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

//#region CswMobilePageHelp

function CswMobilePageHelp(helpDef,$parent,mobileStorage) {
	/// <summary>
	///   Help Page class. Responsible for generating a Mobile help page.
	/// </summary>
    /// <param name="helpDef" type="Object">Help definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageHelp">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    if(isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    var $help = $('<p>Help</p>');

	if (debugOn()) //this is set onLoad based on the includes variable 'debug'
	{
		$help.append('</br></br></br>');
		var $logLevelDiv = $help.CswDiv('init')
								.CswAttrXml({ 'data-role': 'fieldcontain' });
		$('<label for="mobile_log_level">Logging</label>')
								.appendTo($logLevelDiv);

		$logLevelDiv.CswSelect('init', {
										ID: 'mobile_log_level',
										selected: debugOn() ? 'on' : 'off',
										values: [{ value: 'off', display: 'Logging Disabled' },
											{ value: 'on', display: 'Logging Enabled' }],
										onChange: function($select) {
											if ($select.val() === 'on') {
												debugOn(true);
												$('.debug').css('display', '').show();
											} else {
												debugOn(false);
												$('.debug').css('diplay', 'none').hide();
											}
										}
									})
									.CswAttrXml({ 'data-role': 'slider' });

	}
	
    var p = {
	    level: -1,
	    DivId: 'helpdiv',       // required
	    HeaderText: 'Help',
	    theme: 'b',
	    $content: $help,
        onOnlineClick: function () {},
        onRefreshClick: function () {}
	};
	if (helpDef) $.extend(p, helpDef);

    var pageDef = p;
    delete pageDef.onOnlineClick;
    delete pageDef.onRefreshClick;
    
    var onlineValue = mobileStorage.onlineStatus();
    
    if( isNullOrEmpty(pageDef.footerDef)) {
        pageDef.footerDef = {
		    buttons: {
					online: { ID: p.DivId + '_gosyncstatus',
								text: onlineValue,
								cssClass: 'ui-btn-active onlineStatus ' + onlineValue.toLowerCase(),
								dataIcon: 'gear',
					            onClick: p.onOnlineClick
					},
		            refresh: { ID: p.DivId + '_refresh',
								text: 'Refresh',
								cssClass: 'refresh',
								dataIcon: 'refresh',
		                        onClick: p.onRefreshClick
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

	if (isNullOrEmpty(pageDef.headerDef)) {
	    pageDef.headerDef = {
	        buttons: {
	            back: { ID: p.DivId + '_back',
	                text: 'Back',
	                cssClass: 'ui-btn-left',
	                dataDir: 'reverse',
	                dataIcon: 'arrow-l' }
	        }
	    };
	}

	var helpPage = new CswMobilePageFactory(pageDef, $parent);
	var helpHeader = helpPage.mobileHeader;
	var helpFooter = helpPage.mobileFooter;
	var $content = helpPage.$content;
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.mobileHeader = helpHeader;
    this.mobileFooter = helpFooter;
    this.$pageDiv = helpPage.$pageDiv;

    this.onPageOpen = function() {
        this.$pageDiv.CswChangePage({ transition: 'slideup' });
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageHelp