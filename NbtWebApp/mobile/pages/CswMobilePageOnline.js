/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobile.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../sync/CswMobileSync.js" />
/// <reference path="../sync/CswMobileBackgroundTask.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../globals/CswMobileEnums.js" />

//#region CswMobilePageOnline

function CswMobilePageOnline(onlineDef, $parent, mobileStorage, mobileSync, mobileBgTask, $contentRole) {
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

    var pageDef = { };
    var id, title, contentDivId, $content, $onlineBtn, $syncBtn, $logoutBtn, $logBtn,
        divSuffix = '_contpage';
    
    //ctor
    (function () {
        pageDef = {
            level: -1,
            DivId: '',
            title: '',
            theme: CswMobileGlobal_Config.theme,
            buttons: [CswMobileFooterButtons.fullsite, '', CswMobileFooterButtons.help, CswMobileHeaderButtons.back]
        };
        if (mobileStorage.amOnline()) {
            pageDef.buttons[1] = CswMobileFooterButtons.refresh;
        }
        if (onlineDef) {
            $.extend(pageDef, onlineDef);
        }

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.login.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.login.title);
        $content = ensureContent($contentRole, contentDivId);

        getContent();
    })();     //ctor
    
    function getContent() {
        var hideFailure = isNullOrEmpty(mobileStorage.lastSyncAttempt()) ? '' : 'none',
            onlineBtnText = mobileStorage.stayOffline() ? 'Go Online' : 'Go Offline',
            onlineClass = mobileStorage.stayOffline() ? 'offline' : 'online';
        $content = ensureContent($content, contentDivId);
        
        $content.append('<p>Pending Unsynced Changes: <span id="ss_pendingchangecnt">' + tryParseString(mobileStorage.getItem('unSyncedChanges'), '0') + '</span></p>');
        $content.append('<p>Last Sync Success: <span id="ss_lastsync_success">' + mobileStorage.lastSyncSuccess() + '</span></p>');
        $content.append('<p style="display:' + hideFailure + ' ;">Last Sync Failure: <span id="ss_lastsync_attempt">' + mobileStorage.lastSyncAttempt() + '</span></p>');
        $syncBtn = $('<a id="ss_forcesync" data-identity="ss_forcesync" data-url="ss_forcesync" href="javascript:void(0)" data-role="button">Force Sync Now</a>')
                        .appendTo($content)
                        .bind('click', function () {
                            return startLoadingMsg(function () {
                                mobileSync.queueOnSuccess(function() {
                                    stopLoadingMsg();
                                });
                                mobileSync.queueOnError(function () {
                                    stopLoadingMsg();
                                });
                                mobileSync.initSync();
                            });
                        });
        $onlineBtn = $('<a id="ss_gooffline" class="' + onlineClass + '" data-identity="ss_gooffline" data-url="ss_gooffline" href="javascript:void(0)" data-role="button">' + onlineBtnText + '</a>')
                        .appendTo($content)
                        .click( function() {
                            var stayOffline = false === mobileStorage.stayOffline();
                            mobileStorage.stayOffline(stayOffline);
                            toggleOffline(true);
                            return false;
                        });        
        $content.append('<br/>');
        $logoutBtn = $('<a id="ss_logout" data-identity="ss_logout" data-url="ss_logout" href="javascript:void(0)" data-role="button">Logout</a>')
                        .appendTo($content)
                        .bind('click', function() {
                            return startLoadingMsg(function() { onLogout(mobileStorage); });
                        });        
        if (debugOn()) {
            $logBtn = $('<a id="ss_debuglog" class="debug" data-identity="ss_debuglog" data-url="ss_debuglog" href="javascript:void(0)" data-role="button">Start Logging</a>')
                        .appendTo($content)
                        .bind('click', function() {
                            toggleLogging();
                            return false;
                        });
        }
        $contentRole.append($content);
    }
    
    function toggleOffline(doWaitForData) {
        ///<summary>Sets the Go Online/Offline button text.</summary>
        ///<param name="doWaitForData" type="Boolean">True if background task(s) should be restarted.</param>
        if (mobileStorage.amOnline() || $onlineBtn.find('span.ui-btn-text').text() === 'Go Online') {
            setOnline(mobileStorage);
            if (doWaitForData) {
                mobileBgTask.reset();
            }
            $onlineBtn.find('span.ui-btn-text').text('Go Offline');
            $onlineBtn.removeClass('offline').addClass('online');
        }
        else {
            setOffline(mobileStorage);
            if (doWaitForData) {
                mobileBgTask.reset();
            }
            $onlineBtn.find('span.ui-btn-text').text('Go Online');
            $onlineBtn.removeClass('online').addClass('offline');
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

    var SendLogUrl = 'wsNBT.asmx/collectClientLogInfo';
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

//                CswAjaxJson({
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
//			$logDiv.CswChangePage();
        }
    }
    
    function setLastSync(succeeded) {
        if (succeeded) {
            $content.find('#ss_lastsync_success').text(mobileStorage.lastSyncSuccess());
        } else {
            $content.find('#ss_lastsync_attempt').text(mobileStorage.lastSyncAttempt());
        }
    }
    
    //#endregion private
    
    //#region public, priveleged

    return {
        $pageDiv: $parent,
        $contentRole: $contentRole,
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        id: id,
        title: title,
        getContent: getContent,
        setLastSync: setLastSync
    };
    //#endregion public, priveleged
}

//#endregion CswMobilePageOnline