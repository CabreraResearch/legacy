﻿/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobile.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />

//#region CswMobilePageOnline

function CswMobilePageOnline(onlineDef,$parent,mobileStorage,mobileSync,mobileBgTask) {
	/// <summary>
	///   Online Page class. Responsible for generating a Mobile login page.
	/// </summary>
    /// <param name="onlineDef" type="Object">Online definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="mobileSync" type="CswMobileSync">Mobile sync object</param>
    /// <param name="mobileBgTask" type="CswMobileBackgroundTask">Mobile background task object</param>
	/// <returns type="CswMobilePageOnline">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var $content = '';
    var pageDef = { };
    var id = 'syncstatus';
    var title = 'Sync Status';

    var $onlineBtn, $syncBtn, $logoutBtn, $logBtn;
    
    //ctor
    (function() {

        if (isNullOrEmpty(mobileStorage)) {
            mobileStorage = new CswMobileClientDbResources();
        }
        if (isNullOrEmpty(mobileSync)) {
            mobileSync = new CswMobileSync({ }, mobileStorage);
        }
        if (isNullOrEmpty(mobileBgTask)) {
            mobileBgTask = new CswMobileBackgroundTask(mobileStorage, mobileSync, { });
        }

        var p = {
            level: -1,
            DivId: 'syncstatus',       // required
            title: 'Sync Status',
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            theme: CswMobileGlobal_Config.theme,
            onRefreshClick: function() {},
            onHelpClick: function() {}
        };
        if (onlineDef) $.extend(p, onlineDef);

        if(!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        
        if (isNullOrEmpty(p.footerDef.buttons)) {
            p.footerDef.buttons.online = makeFooterButtonDef(CswMobileFooterButtons.online, id, null, mobileStorage);
            p.footerDef.buttons.refresh = makeFooterButtonDef(CswMobileFooterButtons.refresh, id, p.onRefreshClick);
            p.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
            p.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, p.onHelpClick);
        }

        if (isNullOrEmpty(p.headerDef.buttons)) {
            p.headerDef.buttons.back = makeHeaderButtonDef(CswMobileHeaderButtons.back, id);
        }
        
        pageDef = p;
        
        $content = getContent();
    })(); //ctor
    
    function getContent() {
        var content = '<p>Pending Unsynced Changes: <span id="ss_pendingchangecnt">' + tryParseString(mobileStorage.getItem('unSyncedChanges'), '0') + '</span></p>';
        content += '<p>Last Sync Success: <span id="ss_lastsync_success">' + mobileStorage.lastSyncSuccess() + '</span></p>';
        var hideFailure = isNullOrEmpty(mobileStorage.lastSyncAttempt()) ? '' : 'none';
        content += '<p style="display:' + hideFailure + ' ;">Last Sync Failure: <span id="ss_lastsync_attempt">' + mobileStorage.lastSyncAttempt() + '</span></p>';
        content += '<a id="ss_forcesync" data-identity="ss_forcesync" data-url="ss_forcesync" href="javascript:void(0)" data-role="button">Force Sync Now</a>';
        content += '<a id="ss_gooffline" data-identity="ss_gooffline" data-url="ss_gooffline" href="javascript:void(0)" data-role="button">Go Offline</a>';
        content += '<br/>';
        content += '<a id="ss_logout" data-identity="ss_logout" data-url="ss_logout" href="javascript:void(0)" data-role="button">Logout</a>';
        if (debugOn()) {
            content += '<a id="ss_debuglog" class="debug" data-identity="ss_debuglog" data-url="ss_debuglog" href="javascript:void(0)" data-role="button">Start Logging</a>';
        }

        var $online = $(content);
        
        
        $syncBtn = $online.find('#ss_forcesync')
                                .bind('click', function() {
                                    return startLoadingMsg(function() {
                                        mobileSync.initSync();
                                    });
                                });

        $onlineBtn = $online.find('#ss_gooffline')
                                .bind('click', function() {
                                    var stayOffline = !mobileStorage.stayOffline();
                                    mobileStorage.stayOffline(stayOffline);
                                    toggleOffline(true);
                                    return false;
                                });
        $logoutBtn = $online.find('#ss_logout')
                            .bind('click', function() {
                                return startLoadingMsg(function() { onLogout(mobileStorage); });
                            });

        $logBtn = $online.find('#ss_debuglog')
                        .bind('click', function() {
                            toggleLogging();
                            return false;
                        });
        return $online;
    }
    
	function toggleOffline(doWaitForData) {
		///<summary>Changes the Online status style of all mobile online buttons. Sets the Go Online/Offline button text.</summary>
	    ///<param name="doWaitForData" type="Boolean">True if background task(s) should be restarted.</param>
		if (mobileStorage.amOnline() || $onlineBtn.text() === 'Go Online') {
			setOnline();
			if (doWaitForData) {
			    mobileBgTask.reset();
			}
			$onlineBtn.text('Go Offline');
		}
		else {
			setOffline();
			if (doWaitForData) {
				mobileBgTask.reset();
			}
			$onlineBtn.text('Go Online');
		}
	}
	
	function toggleLogging() {
			var logging = !doLogging();
			doLogging(logging);
			if (logging) {
				setStartLog();
			} else {
				setStopLog();
			}
		}

	function setStartLog() {
		if (doLogging()) {
			var logger = new CswProfileMethod('setStartLog');
			cacheLogInfo(logger);
			$('.debug').removeClass('debug-off')
						.addClass('debug-on')
						.find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
						.text('Sync Log')
						.addClass('debug-on')
						.removeClass('debug-off')
						.end();
		}
	}

    var SendLogUrl = '/NbtWebApp/wsNBT.asmx/collectClientLogInfo';
	function setStopLog() {
		if (!doLogging()) {
			$('.debug').removeClass('debug-on')
						.addClass('debug-off')
						.find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
						.text('Sync Log')
						.addClass('debug-off')
						.removeClass('debug-on')
						.end();
			var logger = new CswProfileMethod('setStopLog');
			cacheLogInfo(logger);

			var dataJson = {
				'Context': 'CswMobile',
				'UserName': mobileStorage.username(),
				'CustomerId': mobileStorage.customerid(),
				'LogInfo': mobileStorage.getItem('debuglog')
			};

//                CswAjaxJSON({
//                        url: opts.SendLogUrl,
//                        data: dataJson,
//                        success: function() {
//                            $loggingBtn.removeClass('debug-on')
//                                            .addClass('debug-off')
//                                            .find('span.ui-btn-text') // case 22254: this type of hack is likely to break in the future
//                                            .text('Start Log')
//                                .end();
//                            purgeLogInfo();
//                        }
//                    });

//			var params = {
//				DivId: 'loginfodiv',
//				title: 'Log Info',
//				HideHelpButton: false,
//				HideOnlineButton: false,
//				HideBackButton: false,
//				HideRefreshButton: true,
//				HideSearchButton: true,
//				$content: mobileStorage.getItem('debuglog')
//			};
//			var $logDiv = _addPageDivToBody(params);
//			$logDiv.CswUnbindJqmEvents();
//			$logDiv.CswBindJqmEvents(params);
//			$logDiv.CswChangePage();
		}
	}
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageOnline