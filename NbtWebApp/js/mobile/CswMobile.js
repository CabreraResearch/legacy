/// <reference path="../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../thirdparty/jquery/core/jquery.mobile/jquery.mobile-1.0b1.js" />
/// <reference path="../_Global.js" />
/// <reference path="../mobile/clientdb/CswMobileClientDb.js" />
/// <reference path="../mobile/clientdb/CswMobileClientDbResources.js" />
/// <reference path="../CswClientDb.js" />
/// <reference path="../CswEnums.js" />
/// <reference path="../CswProfileMethod.js" />
/// <reference path="../mobile/CswMobileTools.js" />
/// <reference path="../mobile/sync/CswMobileBackgroundTask.js" />
/// <reference path="../mobile/sync/CswMobileSync.js" />
/// <reference path="controls/CswMobileMenuButton.js" />
/// <reference path="controls/ICswMobileWebControls.js" />
/// <reference path="controls/CswMobilePageFooter.js" />
/// <reference path="controls/CswMobilePageHeader.js" />
/// <reference path="pages/CswMobilePageFactory.js" />
/// <reference path="objectclasses/CswMobileNodesFactory.js" />

//var profiler = $createProfiler();

CswAppMode.mode = 'mobile';

;(function($) {
	/// <param name="$" type="jQuery" />

	$.fn.CswMobile = function(options) {
		/// <summary>
		///   Generates the Nbt Mobile page
		/// </summary>

		//#region Resource Initialization
		
		var x = {
			//UpdateViewUrl: '/NbtWebApp/wsNBT.asmx/UpdateProperties',
			MainPageUrl: '/NbtWebApp/Mobile.html',
			Theme: CswMobileGlobal_Config.theme,
			PollingInterval: 30000, //30 seconds
			RandomConnectionFailure: false
		};

		if (options) {
			$.extend(x, options);
		}
		
		var mobileStorage = new CswMobileClientDbResources(); 
		
		debugOn(debug);
		
		var forMobile = true;
		
		var sessionId = mobileStorage.sessionid();
		if(isNullOrEmpty(sessionId)) {
			Logout(mobileStorage);
		}

		var storedViews = mobileStorage.getItem('storedViews');
		
		var mobileSyncOptions = {
			onSync: processModifiedNodes,
			onSuccess: processUpdatedNodes,
			onComplete: function () {
				updatedUnsyncedChanges();
			},
			ForMobile: true
		};

		var mobileSync = new CswMobileSync(mobileSyncOptions, mobileStorage);

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
		
		//#endregion Resource Initialization
		
		var loginPage, viewsPage, offlinePage, helpPage, onlinePage;

		// case 20355 - error on browser refresh
		if (!isNullOrEmpty(sessionId)) {
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
		    
		    var loginDef = {
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
		    loginPage = new CswMobilePageFactory(CswMobilePage_Type.login, loginDef, $('body'));
			return loginPage;
		}
	    
	    function makeOfflinePage() {
			///<summary>Create a Mobile offline (Sorry Charlie) page</summary>
		    ///<returns type="CswMobilePageOffline">CswMobilePageOffline page.</returns>
		    var offlineDef = {
				theme: x.Theme,
			    onHelpClick: onHelpClick,
		        mobileStorage: mobileStorage
			};
		    offlinePage = new CswMobilePageFactory(CswMobilePage_Type.offline, offlineDef, $('body'));
			return offlinePage;
		}

	    function makeOnlinePage() {
            ///<summary>Create a Mobile online (Sync Status) page</summary>
	        ///<returns type="CswMobilePageOnline">CswMobilePageOnline page.</returns>
		    var syncDef = {
                theme: x.Theme,
		        onRefreshClick: onRefreshClick,
                onHelpClick: onHelpClick,
		        mobileStorage: mobileStorage,
		        mobileSync: mobileSync
		    };
		    onlinePage = new CswMobilePageFactory(CswMobilePage_Type.online, syncDef, $('body') );
		    return onlinePage;
		}
	    
	    function makeHelpPage() {
			///<summary>Create a Mobile help page</summary>
	        ///<returns type="CswMobilePageHelp">CswMobilePageHelp page.</returns>
	        var helpDef = {
                theme: x.Theme,
			    onOnlineClick: onOnlineClick,
			    onRefreshClick: onRefreshClick,
	            mobileStorage: mobileStorage
		    };
		    helpPage = new CswMobilePageFactory(CswMobilePage_Type.help, helpDef, $('body') );
			return helpPage;
		}
	    
	    function makeSearchPage() {
			///<summary>Create a Mobile search page</summary>
	        ///<returns type="CswMobilePageSearch">CswMobilePageSearch page.</returns>
	        var searchDef = {
                ParentId: mobileStorage.currentViewId(),
			    theme: x.Theme,
			    onOnlineClick: onOnlineClick,
	            mobileStorage: mobileStorage
		    };
	        var searchPage = new CswMobilePageFactory(CswMobilePage_Type.search, searchDef, $('body') );
			return searchPage;
		}

	    //#endregion Static Page Creation
	    
	    //#region Dynamic Page Creation
	    
		function makeViewsPage() {
			///<summary>Create a Mobile views page</summary>
		    ///<returns type="CswMobilePageViews">CswMobilePageViews page.</returns>
		    var viewsDef = {
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
		    viewsPage = new CswMobilePageFactory(CswMobilePage_Type.views, viewsDef, $('body') );
			return viewsPage;
		}
		
	    function makeNodesPage(opts) {
	        ///<summary>Create a Mobile nodes page</summary>
		    ///<returns type="CswMobilePageNodes">CswMobilePageNodes page.</returns>
		    var nodesDef = {
		        ParentId: '',
		        DivId: '',
		        level: 1,
		        json: '',
		        theme: x.Theme,
		        onHelpClick: onHelpClick,
		        onOnlineClick: onOnlineClick,
		        onRefreshClick: onRefreshClick,
		        mobileStorage: mobileStorage,
		        onListItemSelect: function(param) {
		            var tabsPage = makeTabsPage(param);
		            tabsPage.CswChangePage();
		        }
		    };
	        if(opts) {
	            $.extend(nodesDef, opts);
	        }
		    var nodesPage = new CswMobilePageFactory(CswMobilePage_Type.nodes, nodesDef, $('body') );
			return nodesPage;
	    }
	    
	    function makeTabsPage(opts) {
	        ///<summary>Create a Mobile tabs page</summary>
		    ///<returns type="CswMobilePageTabs">CswMobilePageTabs page.</returns>
		    var tabsDef = {
		        ParentId: '',
		        DivId: '',
		        level: 1,
		        json: '',
		        theme: x.Theme,
		        onHelpClick: onHelpClick,
		        onOnlineClick: onOnlineClick,
		        onRefreshClick: onRefreshClick,
		        mobileStorage: mobileStorage,
		        onListItemSelect: function(param) {
		            var propsPage = makePropsPage(param);
		            propsPage.CswChangePage();
		        }
		    };
	        if(opts) {
	            $.extend(tabsDef, opts);
	        }
		    var tabsPage = new CswMobilePageFactory(CswMobilePage_Type.tabs, tabsDef, $('body') );
			return tabsPage;
	    }
	    
	    function makePropsPage(opts) {
	        ///<summary>Create a Mobile nodes page</summary>
		    ///<returns type="CswMobilePageViews">CswMobilePageViews page.</returns>
		    var propsDef = {
		        ParentId: '',
		        DivId: '',
		        level: 1,
		        json: '',
		        theme: x.Theme,
		        onHelpClick: onHelpClick,
		        onOnlineClick: onOnlineClick,
		        onRefreshClick: onRefreshClick,
		        mobileStorage: mobileStorage,
		        onListItemSelect: function(param) {
		            var nextPropsPage = makePropsPage(param);
		            nextPropsPage.CswChangePage();
		        },
		        onPropChange: function(param) {
		            resetPendingChanges();
		            mobileBgTask.reset();
		        }
		    };
	        if(opts) {
	            $.extend(propsDef, opts);
	        }
		    var propsPage = new CswMobilePageFactory(CswMobilePage_Type.props, propsDef, $('body') );
			return propsPage;
	    }
	    
	    //#endregion Dynamic Page Creation
	    
		//#region Button Bindings
		
		function onRefreshClick() {
			///<summary>Event to fire on 'Refresh' button click.</summary>
		    var divId = mobileStorage.currentViewId();
			if(isNullOrEmpty(divId)) {
				window.location.reload();
			}
			else if (mobileStorage.amOnline() && 
				mobileStorage.checkNoPendingChanges() ) {
				
				if(divId === 'viewsdiv') {
					window.location.reload();
				}
				else {
					var jsonData = {
						SessionId: sessionId,
						ParentId: divId,
						ForMobile: forMobile
					};

					CswAjaxJSON({
							formobile: forMobile,
							url: x.ViewUrl,
							data: jsonData,
							stringify: false,
							onloginfail: function(text) { onLoginFail(text, mobileStorage); },
							success: function(data) {
								setOnline(mobileStorage);
								if( !isNullOrEmpty(data['nodes']) ) {
									var viewJSON = data['nodes'];
									
									var params = {
										ParentId: 'viewsdiv',
										DivId: divId,
										title: title,
										json: mobileStorage.updateStoredViewJson(divId, viewJSON),
										parentlevel: 0,
										level: 1,
										HideRefreshButton: false,
										HideSearchButton: false,
										HideBackButton: false
									};
									params.onPageShow = function() { return _loadDivContents(params); };
									_loadDivContents(params).CswChangePage();
								}
							}, // success
							error: function() {
								onError();
							}
						});
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
  		    $('#ss_pendingchangecnt').text( tryParseString(mobileStorage.getItem('unSyncedChanges'),'0') );
		}

	    function resetPendingChanges(succeeded) {
			if ( mobileStorage.pendingChanges() ) {
				$('.onlineStatus').addClass('pendingchanges')
								  .find('span.ui-btn-text')
								  .addClass('pendingchanges');
			} else {
				$('.onlineStatus').removeClass('pendingchanges')
								  .find('span.ui-btn-text')
								  .removeClass('pendingchanges');
			}
			
			if(arguments.length === 1) {
				if (succeeded) {
					mobileStorage.clearUnsyncedChanges();
					updatedUnsyncedChanges();
					$('#ss_lastsync_success').text(mobileStorage.lastSyncSuccess());
				}
				else {
					$('#ss_lastsync_attempt').text(mobileStorage.lastSyncAttempt());
				}
			}
		}
	    
		function processModifiedNodes(onSuccess) {
			if(!isNullOrEmpty(onSuccess)) {
				var modified = false;
				if (isNullOrEmpty(storedViews))
				{
					storedViews = mobileStorage.getItem('storedviews');
				}
				if (!isNullOrEmpty(storedViews))
				{
					for (var viewid in storedViews)
					{
						var view = mobileStorage.getItem(viewid);
						if (!isNullOrEmpty(view))
						{
							for (var nodeId in view['json'])
							{
								var node = mobileStorage.getItem(nodeId);
								if (!isNullOrEmpty(node) && node['wasmodified'])
								{
									modified = true;
									onSuccess(nodeId, node);
								}
							}
						}
					}
					if (!modified)
					{
						onSuccess();
					}
				}
			} else {
				resetPendingChanges(true);
			}
		}
		
		function processUpdatedNodes(data,objectId,objectJSON,isBackgroundTask) {
			if( !isNullOrEmpty(data) ) {
			    setOnline(mobileStorage);
			    var completed = isTrue(data['completed']);
			    var isView = !isNullOrEmpty(data['nodes']);
			    if (isView)
			    {
			        var json = data['nodes'];
			        mobileStorage.updateStoredViewJson(objectId, json, false);
			    } else if (!completed)
			    {
			        mobileStorage.updateStoredNodeJson(objectId, objectJSON, false);
			    }

			    resetPendingChanges(true);

			    if (completed && !isView)
			    {
			        mobileStorage.deleteNode(objectId, objectJSON['viewid']);
			        if (!isBackgroundTask)
			        {
			            $('#' + objectJSON['viewid']).CswChangePage();
			        }
			    }
			}
		}
		
		//#endregion Synchronization
		
		// For proper chaining support
		return this;
	};
})(jQuery);