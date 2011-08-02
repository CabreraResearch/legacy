/// <reference path="../../_Global.js" />
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
    /// <param name="onlineDef" type="Object">Login definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="mobileSync" type="CswMobileSync">Mobile sync object</param>
    /// <param name="mobileBgTask" type="CswMobileBackgroundTask">Mobile background task object</param>
	/// <returns type="CswMobilePageOnline">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    if(isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    if(isNullOrEmpty(mobileSync)) {
        mobileSync = new CswMobileSync({}, mobileStorage);
    }
    if(isNullOrEmpty(mobileBgTask)) {
        mobileBgTask = new CswMobileBackgroundTask(mobileStorage,mobileSync, {});
    }
    
    var content = '<p>Pending Unsynced Changes: <span id="ss_pendingchangecnt">' + tryParseString(mobileStorage.getItem('unSyncedChanges'),'0') + '</span></p>';
	content += '<p>Last Sync Success: <span id="ss_lastsync_success">' + mobileStorage.lastSyncSuccess() + '</span></p>';
	var hideFailure = isNullOrEmpty(mobileStorage.lastSyncAttempt()) ? '' : 'none';
	content += '<p style="display:' + hideFailure + ' ;">Last Sync Failure: <span id="ss_lastsync_attempt">' + mobileStorage.lastSyncAttempt() + '</span></p>';
	content += '<a id="ss_forcesync" data-identity="ss_forcesync" data-url="ss_forcesync" href="javascript:void(0)" data-role="button">Force Sync Now</a>';
	content += '<a id="ss_gooffline" data-identity="ss_gooffline" data-url="ss_gooffline" href="javascript:void(0)" data-role="button">Go Offline</a>';
	content += '<br/>';
	content += '<a id="ss_logout" data-identity="ss_logout" data-url="ss_logout" href="javascript:void(0)" data-role="button">Logout</a>';
	if( debugOn() ) {
		content += '<a id="ss_debuglog" class="debug" data-identity="ss_debuglog" data-url="ss_debuglog" href="javascript:void(0)" data-role="button">Start Logging</a>';
	}
	
    var p = {
		level: -1,
		DivId: 'syncstatus',       // required
		HeaderText: 'Sync Status',
        onSuccess: function() {},
        theme: 'b',
        $content: $(content)
    };
    if(onlineDef) $.extend(p, onlineDef);

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
		            },
					help: { ID: p.DivId + '_help',
								text: 'Help',
								dataIcon: 'info' 
					}
				}
		};
    }
    
    if( isNullOrEmpty(p.headerDef)) {
        p.headerDef = {
		    buttons: {
					back: { ID: p.DivId + '_back',
								text: 'Back',
								cssClass: 'ui-btn-left',
								dataDir: 'reverse',
								dataIcon: 'arrow-l' }
				}
		};
    }
    
    var onlinePage = new CswMobilePageFactory(p, $parent);
    var onlineHeader = onlinePage.mobileHeader;
    var onlineFooter = onlinePage.mobileFooter;
    var $content = onlinePage.$content;
	
	$content.find('#ss_forcesync')
			.bind('click', function() {
				return startLoadingMsg( function () {
						mobileSync.initSync();
				});
			})
			.end()
			.find('#ss_gooffline')
			.bind('click', function() {
				var stayOffline = !mobileStorage.stayOffline();
				mobileStorage.stayOffline(stayOffline);
				_toggleOffline(true);
				return false;
			})
			.end()
			.find('#ss_logout')
			.bind('click', function() {
				return startLoadingMsg( function () { onLogout(mobileStorage); });
			})
			.end()
			.find('#ss_debuglog')
			.bind('click', function() {
				_toggleLogging();
				return false;
			})
			.end();
    
    
	function _toggleOffline(doWaitForData) {
		///<summary>Changes the Online status style of all mobile online buttons. Sets the Go Online/Offline button text.</summary>
	    ///<param name="doWaitForData" type="Boolean">True if background task(s) should be restarted.</param>
	    var $onlineBtn = onlinePage.mobileHeader.online.$control.find('span.ui-btn-text');
		if (mobileStorage.amOnline() || $onlineBtn.text() === 'Go Online') {
			setOnline();
			if (doWaitForData) {
			    mobileBgTask.reset();
			}
			$onlineBtn.text('Go Offline');
			$('.refresh').each(function(){
				var $this = $(this);
				$this.css({'display': ''}).show();
			});
		}
		else {
			setOffline();
			if (doWaitForData) {
				mobileBgTask.reset();
			}
			$onlineBtn.text('Go Online');
			$('.refresh').each(function(){
				var $this = $(this);
				$this.css({'display': 'none'}).hide();
			});
		}
	}
	
	function _toggleLogging() {
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
//				HeaderText: 'Log Info',
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
    this.mobileHeader = onlineHeader;
    this.mobileFooter = onlineFooter;
    this.$pageDiv = onlinePage.$pageDiv;

    this.toggleOffline = _toggleOffline;

    this.onPageOpen = function() {
        this.$pageDiv.CswChangePage({ transition: 'slideup' });
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageOnline