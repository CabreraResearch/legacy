
(function($) {
    /// <param name="$" type="jQuery" />

    $.fn.CswMobile = function(options) {
        /// <summary>
        ///   Generates the Nbt Mobile page
        /// </summary>
        //#region Resource Initialization
        var x = {
            Theme: CswMobileGlobal_Config.theme,
            PollingInterval: 30000 //30 seconds
        };
        if (options) {
            $.extend(x, options);
        }
        
        var mobileStorage = new CswMobileClientDbResources(),
            forMobile = true,
            sessionId = mobileStorage.sessionid();
        
        if(isNullOrEmpty(sessionId)) {
            Logout(mobileStorage);
        }

        var storedViews = mobileStorage.getItem(CswMobileGlobal_Config.storedViews);
        
        var mobileSyncOptions = {
            onSync: processModifiedNodes,
            onSuccess: [ processUpdatedNodes, function () { updatedUnsyncedChanges(); } ],
            onError: [ onError, function () { updatedUnsyncedChanges(); } ],
            ForMobile: true
        };

        var mobileSync = CswMobileSync(mobileSyncOptions, mobileStorage);
        
        var mobileBackgroundTaskOptions = {
            onSuccess: function () {
                setOnline(mobileStorage);
            },
            onError: function () {
                setOffline();
            },
            onLoginFailure: onLoginFail,
            PollingInterval: x.PollingInterval,
            ForMobile: forMobile
        };

        var mobileBgTask = new CswMobileBackgroundTask(mobileStorage, mobileSync, mobileBackgroundTaskOptions);

        var mobilePageFactory = CswMobilePageFactory(x.Theme, mobileStorage, $('body'));
        
        //#endregion Resource Initialization
        
        var loginPage = null, viewsPage = null, offlinePage, helpPage, onlinePage;

        // case 20355 - error on browser refresh
        if (false === isNullOrEmpty(sessionId)) {
            if (isNullOrEmpty(viewsPage)) {
                viewsPage = makeViewsPage();
            }
            viewsPage.CswSetPath();
            mobileStorage.setItem('refreshPage', CswMobilePage_Type.views.id);
        } else {
            if (isNullOrEmpty(loginPage)) {
                loginPage = makeLoginPage();
            }
            mobileBgTask.start(
                function() {
                    // online
                    if (isNullOrEmpty(mobileStorage.sessionid())) {
                        if (isNullOrEmpty(loginPage)) {
                            loginPage = makeLoginPage();
                        }
                        loginPage.CswSetPath();
                        mobileStorage.setItem('refreshPage', CswMobilePage_Type.login.id);
                        loginPage.CswChangePage();
                    }
                },
                function() {
                    // offline
                    if (isNullOrEmpty(offlinePage) ) {
                        offlinePage = makeOfflinePage();
                    }
                    offlinePage.CswSetPath();
                    mobileStorage.setItem('refreshPage', CswMobilePage_Type.offline.id );
                    offlinePage.CswChangePage();
                }
            );
        }
        
        //#region Static Page Creation
        
        function makeLoginPage() {
            ///<summary>Create a Mobile login page</summary>
            ///<returns type="CswMobilePageLogin">CswMobilePageLogin page.</returns>
            var pageDef = {
                theme: x.Theme,
                onHelpClick: onHelpClick,
                onSuccess: function (data,userName,accessId) {
                    startLoadingMsg();
                    sessionId = $.CswCookie('get', CswCookieName.SessionId);
                    mobileStorage.sessionid(sessionId);
                    mobileStorage.username(userName); 
                    mobileStorage.customerid(accessId);
                    viewsPage = makeViewsPage();
                    viewsPage.CswChangePage();
                    setTimeout(function() {
                        loginPage = loginPage.remove();  
                    }, 10000);
                },
                mobileStorage: mobileStorage
            };
            loginPage = mobilePageFactory.makePage(CswMobilePage_Type.login, pageDef);
            return loginPage;
        }
        
        function makeOfflinePage() {
            ///<summary>Create a Mobile offline (Sorry Charlie) page</summary>
            ///<returns type="CswMobilePageOffline">CswMobilePageOffline page.</returns>
            var pageDef = {
                theme: x.Theme,
                onHelpClick: onHelpClick,
                mobileStorage: mobileStorage
            };
            offlinePage = mobilePageFactory.makePage(CswMobilePage_Type.offline, pageDef);
            return offlinePage;
        }

        function makeOnlinePage() {
            ///<summary>Create a Mobile online (Sync Status) page</summary>
            ///<returns type="CswMobilePageOnline">CswMobilePageOnline page.</returns>
            var pageDef = {
                theme: x.Theme,
                onRefreshClick: onRefreshClick,
                onHelpClick: onHelpClick,
                mobileStorage: mobileStorage,
                mobileSync: mobileSync,
                mobileBgTask: mobileBgTask
            };
            onlinePage = mobilePageFactory.makePage(CswMobilePage_Type.online, pageDef);
            return onlinePage;
        }
        
        function makeHelpPage() {
            ///<summary>Create a Mobile help page</summary>
            ///<returns type="CswMobilePageHelp">CswMobilePageHelp page.</returns>
            var pageDef = {
                theme: x.Theme,
                onOnlineClick: onOnlineClick,
                onRefreshClick: onRefreshClick,
                mobileStorage: mobileStorage
            };
            helpPage = mobilePageFactory.makePage(CswMobilePage_Type.help, pageDef);
            return helpPage;
        }
        
        function makeSearchPage() {
            ///<summary>Create a Mobile search page</summary>
            ///<returns type="CswMobilePageSearch">CswMobilePageSearch page.</returns>
            var pageDef = {
                ParentId: mobileStorage.currentViewId(),
                theme: x.Theme,
                onOnlineClick: onOnlineClick,
                mobileStorage: mobileStorage,
                onListItemSelect: function(param) {
                    var tabsPage = makeTabsPage(param);
                    tabsPage.CswChangePage();
                }
            };
            var searchPage = mobilePageFactory.makePage(CswMobilePage_Type.search, pageDef);
            return searchPage;
        }

        //#endregion Static Page Creation
        
        //#region Dynamic Page Creation
        
        function makeViewsPage() {
            ///<summary>Create a Mobile views page</summary>
            ///<returns type="CswMobilePageViews">CswMobilePageViews page.</returns>
            var pageDef = {
                theme: x.Theme,
                onHelpClick: onHelpClick,   
                onOnlineClick: onOnlineClick,
                onRefreshClick: onRefreshClick,
                mobileStorage: mobileStorage,
                onListItemSelect: function(opts) {
                    var nodePage = makeNodesPage(opts);
                    nodePage.CswChangePage();
                }
            };
            viewsPage = mobilePageFactory.makePage(CswMobilePage_Type.views, pageDef);
            return viewsPage;
        }
        
        function makeNodesPage(opts) {
            ///<summary>Create a Mobile nodes page</summary>
            ///<returns type="CswMobilePageNodes">CswMobilePageNodes page.</returns>
            var pageDef = {
                ParentId: '',
                DivId: '',
                level: 1,
                json: '',
                theme: x.Theme,
                onHelpClick: onHelpClick,
                onOnlineClick: onOnlineClick,
                onRefreshClick: onRefreshClick,
                onSearchClick: onSearchClick,
                mobileStorage: mobileStorage,
                onListItemSelect: function(param) {
                    var nodeJson = mobileStorage.fetchCachedNodeJson(param.nodeId),
                        nextPage;
                    if(isNullOrEmpty(nodeJson)) {
                        nodeJson = param.cachedJson;
                    }
                    if (contains(nodeJson, 'tabs')) {
                        nextPage = makeTabsPage(param);
                    } else {
                        nextPage = makeNodesPage(param);
                    }
                    nextPage.CswChangePage();
                }
            };
            if(opts) {
                $.extend(pageDef, opts);
            }
            var nodesPage = mobilePageFactory.makePage(CswMobilePage_Type.nodes, pageDef);
            return nodesPage;
        }
        
        function makeTabsPage(opts) {
            ///<summary>Create a Mobile tabs page</summary>
            ///<returns type="CswMobilePageTabs">CswMobilePageTabs page.</returns>
            var pageDef = {
                ParentId: '',
                DivId: '',
                level: 1,
                json: '',
                theme: x.Theme,
                onHelpClick: onHelpClick,
                onOnlineClick: onOnlineClick,
                onRefreshClick: onRefreshClick,
                onSearchClick: onSearchClick,
                mobileStorage: mobileStorage,
                onListItemSelect: function(param) {
                    var propsPage = makePropsPage(param);
                    propsPage.CswChangePage();
                }
            };
            if(opts) {
                $.extend(pageDef, opts);
            }
            var tabsPage = mobilePageFactory.makePage(CswMobilePage_Type.tabs, pageDef);
            return tabsPage;
        }
        
        function makePropsPage(opts) {
            ///<summary>Create a Mobile nodes page</summary>
            ///<returns type="CswMobilePageViews">CswMobilePageViews page.</returns>
            var pageDef = {
                ParentId: '',
                DivId: '',
                level: 1,
                json: '',
                theme: x.Theme,
                onHelpClick: onHelpClick,
                onOnlineClick: onOnlineClick,
                onRefreshClick: onRefreshClick,
                onSearchClick: onSearchClick,
                mobileStorage: mobileStorage,
                onListItemSelect: function(param) {
                    var nextPropsPage = makePropsPage(param);
                    nextPropsPage.CswChangePage();
                },
                onPropChange: function() {
                    mobileSync.initSync();
                    resetPendingChanges();
                    mobileBgTask.reset();
                }
            };
            if(opts) {
                $.extend(pageDef, opts);
            }
            var propsPage = mobilePageFactory.makePage(CswMobilePage_Type.props, pageDef);
            return propsPage;
        }
        
        //#endregion Dynamic Page Creation
        
        //#region Button Bindings
        
        function onRefreshClick() {
            ///<summary>Event to fire on 'Refresh' button click.</summary>
            var divId = $.mobile.activePage.CswAttrDom('id');
            mobileStorage.forceContentRefresh(true);
            if(isNullOrEmpty(divId)) {
                window.location.reload();
            }
            else if (mobileStorage.amOnline() && 
                mobileStorage.checkNoPendingChanges() ) {
                
                if(divId === 'viewsdiv') {
                    window.location.reload();
                }
                else {
                    viewsPage = makeViewsPage();
                    viewsPage.CswChangePage();
                }
            }
        }

        function onOnlineClick() {
            ///<summary>Event to fire on 'Online' button click.</summary>
            onlinePage = makeOnlinePage();
            onlinePage.$pageDiv.CswChangePage();
        }
        
        function onHelpClick() {
            ///<summary>Event to fire on 'Help' button click.</summary>
            helpPage = makeHelpPage();
            helpPage.$pageDiv.CswChangePage();
        }
        
        function onSearchClick() {
            ///<summary>Event to fire on 'Search' button click.</summary>
            var searchPage = makeSearchPage();
            searchPage.$pageDiv.CswChangePage();
        }
        
        //#endregion Button Bindings
        
        //#region Synchronization

        function updatedUnsyncedChanges() {
            ///<summary> Updates the count of unsynced changes on the Online page.</summary>
            if (false === isNullOrEmpty(onlinePage)) {
                var pendingChngCnt = onlinePage.$content.find('#ss_pendingchangecnt');
                if (!isNullOrEmpty(pendingChngCnt)) {
                    pendingChngCnt.text(tryParseString(mobileStorage.getItem('unSyncedChanges'), '0'));
                }
            }
        }

        function resetPendingChanges(succeeded) {
            if (arguments.length === 1) {
                if (succeeded) {
                    mobileStorage.clearUnsyncedChanges();
                    updatedUnsyncedChanges();
                }
                if (false === isNullOrEmpty(onlinePage)) {
                    onlinePage.page.setLastSync(succeeded);
                }
            }
            
            if (mobileStorage.pendingChanges()) {
                $('.' + CswMobileCssClasses.onlineStatus.name)
                    .addClass(CswMobileCssClasses.pendingChanges.name)
                    .find('span.ui-btn-text')
                    .addClass(CswMobileCssClasses.pendingChanges.name);
            } else {
                $('.' + CswMobileCssClasses.onlineStatus.name)
                    .removeClass(CswMobileCssClasses.pendingChanges.name)
                    .find('span.ui-btn-text')
                    .removeClass(CswMobileCssClasses.pendingChanges.name);
            }
        }
        
        function processModifiedNodes(onSuccess) {
            var modified = false, 
                viewid, view, node, nodeId;
            if(isFunction(onSuccess)) {
                if (isNullOrEmpty(storedViews)) {
                    storedViews = mobileStorage.getItem(CswMobileGlobal_Config.storedViews);
                }
                if (false === isNullOrEmpty(storedViews)) {
                    for (viewid in storedViews) {
                        view = mobileStorage.getItem(viewid);
                        if (false === isNullOrEmpty(view)) {
                            for (nodeId in view.json) {
                                node = mobileStorage.getItem(nodeId);
                                if (false === isNullOrEmpty(node) && node.wasmodified) {
                                    modified = true;
                                    onSuccess(nodeId, node);
                                }
                            }
                        }
                    }
                    if (false === modified) {
                        onSuccess();
                    }
                }
            } else {
                resetPendingChanges(true);
            }
            stopLoadingMsg();
        }
        
        function processUpdatedNodes(data,objectId,objectJson,isBackgroundTask) {
            var isView, isNode, json, completed, $viewPage;
            if (false === isNullOrEmpty(data)) {
                setOnline(mobileStorage);
                completed = isTrue(data.completed);
                isView = contains(data, 'nodes') && false === isNullOrEmpty(data.nodes);
                isNode = contains(data, objectId) && false === isNullOrEmpty(data[objectId]);
                if (isView) {
                    json = data.nodes;
                    mobileStorage.updateStoredViewJson(objectId, json, false);
                } else if (isNode && false === completed) {
                    json = data[objectId];
                    mobileStorage.updateStoredNodeJson(objectId, json, false);
                }

                resetPendingChanges(true);

                if (completed && false === isView) {
                    mobileStorage.deleteNode(objectId, objectJson.viewid);
                    if (false === isBackgroundTask) {
                        mobileStorage.forceContentRefresh(true);
                        $viewPage = $('#' + mobileStorage.currentViewId());
                        $('#' + objectId + '_li').remove();
                        $viewPage.CswChangePage();
                    }
                }
            }
        }
        
        //#endregion Synchronization
        
        // For proper chaining support
        return this;
    };
})(jQuery);