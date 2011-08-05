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

//var profiler = $createProfiler();

CswAppMode.mode = 'mobile';

;(function($) {
	/// <param name="$" type="jQuery" />

	$.fn.CswMobile = function(options) {
		/// <summary>
		///   Generates the Nbt Mobile page
		/// </summary>
		var $body = this;

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
		
	    //#region Page Creation
	    
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
		            loginPage = loginPage.remove();
		        },
		        mobileStorage: mobileStorage
		    };
		    loginPage = new CswMobilePageFactory(CswMobilePage_Type.login, loginDef, $body);
			return loginPage;
		}

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
		    viewsPage = new CswMobilePageFactory(CswMobilePage_Type.views, viewsDef, $body );
			return viewsPage;
		}
		
		function makeOfflinePage() {
			///<summary>Create a Mobile offline (Sorry Charlie) page</summary>
		    ///<returns type="CswMobilePageOffline">CswMobilePageOffline page.</returns>
		    var offlineDef = {
				theme: x.Theme,
			    onHelpClick: onHelpClick,
		        mobileStorage: mobileStorage
			};
		    offlinePage = new CswMobilePageFactory(CswMobilePage_Type.offline, offlineDef, $body);
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
		    onlinePage = new CswMobilePageFactory(CswMobilePage_Type.online, syncDef, $body );
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
		    helpPage = new CswMobilePageFactory(CswMobilePage_Type.help, helpDef, $body );
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
	        var searchPage = new CswMobilePageFactory(CswMobilePage_Type.search, searchDef, $body );
			return searchPage;
		}
	    
	    function makeNodesPage(opts) {
	        ///<summary>Create a Mobile nodes page</summary>
		    ///<returns type="CswMobilePageViews">CswMobilePageViews page.</returns>
		    var nodesDef = {
		        ParentId: '',
		        DivId: '',
		        level: 1,
		        json: '',
		        theme: x.Theme,
		        onHelpClick: onHelpClick,
		        onOnlineClick: onOnlineClick,
		        onRefreshClick: onRefreshClick,
		        mobileStorage: mobileStorage
		    };
	        if(opts) {
	            $.extend(nodesDef, opts);
	        }
		    var nodesPage = new CswMobilePageFactory(CswMobilePage_Type.nodes, nodesDef, $body );
			return nodesPage;
	    }
	    
	    //#endregion Page Creation
	    
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
	    
	    // ------------------------------------------------------------------------------------
		// List items fetching
		// ------------------------------------------------------------------------------------

		function _loadDivContents(params) {
			var logger = new CswProfileMethod('loadDivContents');
			
			if($('#logindiv')) $('#logindiv').remove();
			
			
			mobileBgTask.reset();
			
			var p = {
				ParentId: '',
				level: 1,
				DivId: '',
				title: '',
				HideRefreshButton: false,
				HideSearchButton: false,
				json: '',
				SessionId: sessionId,
				PageType: 'search'
			};
			if (params) $.extend(p, params);

			var viewId = (p.level < 2) ? mobileStorage.currentViewId(p.DivId) : mobileStorage.currentViewId();
	   
			var $retDiv = $('#' + p.DivId);

			if (isNullOrEmpty($retDiv) || $retDiv.length === 0 || $retDiv.find('div:jqmData(role="content")').length === 1) {
				switch(p.level) {
					case 0: //views
					    var viewsPage = makeViewsPage();
					    $retDiv = viewsPage.$pageDiv;
					    break;
				    case 1: //nodes
				        var nodesPage = makeNodesPage({
				                ParentId: viewId,
				                DivId: p.DivId,
				                level: p.level
				            });
				        $retDiv = nodesPage.$pageDiv;
				        break;
				    default: // Level 2 and up
					    var cachedJson = mobileStorage.fetchCachedNodeJson(p.DivId);
					    p.PageType = 'tab';
					    if( !isNullOrEmpty(cachedJson) ) {
						    p.json = cachedJson['subitems'];

						    if (!isNullOrEmpty(p.json)) {
							    $retDiv = _loadDivContentsJson(p);
						    }
					    } else {
						    stopLoadingMsg();
					    }
				        break;
				}
			}
			cacheLogInfo(logger);
			return $retDiv;
		} // _loadDivContents()

		function _loadDivContentsJson(params) {
			params.parentlevel = params.level;
			var $retDiv = _processViewJson(params);
			return $retDiv;
		}
        
		function _processViewJson(params) {
			var logger = new CswProfileMethod('processViewJson');
			var p = {
				ParentId: '',
				DivId: '',
				title: '',
				json: '',
				parentlevel: '',
				PageType: '',
				level: '',
				HideSearchButton: false,
				HideOnlineButton: false,
				HideRefreshButton: false,
				HideHelpButton: false,
				HideBackButton: false,
				nextTab: ''
			};
			if (params) $.extend(p, params);

		    var $retDiv = $('#' + p.DivId);

			var $content = $retDiv.find('div:jqmData(role="content")').empty();

			var showLoading = (p.PageType !== 'prop');
			var $list = $content.cswUL({cssclass: 'csw_listview', showLoading: showLoading});
			
			for(var key in p.json)
			{
				var item = { };
				$.extend(item, p);
				item.json = { id: key, value: p.json[key] };
				_makeListItemFromJson($list, item)
					.CswAttrXml('data-icon', false) //hides the arrow
					.appendTo($list);
			}

			if( !isNullOrEmpty(p.nextTab) ) {
				var item = { };
				$.extend(item, p);
				item.json = { id: p.nextTab };
				item.PageType = 'tab';
				item.DivId = item.ParentId;
				item.suppressProps = true;
				_makeListItemFromJson($list, item)
					//.CswAttrXml('data-icon', true) //show the arrow
					.appendTo($list);
			}
			
			logger.setAjaxSuccess();
			
			resetPendingChanges();
			
			if(!mobileStorage.stayOffline()) {
			    toggleOnline(mobileStorage);
			}
			cacheLogInfo(logger);

			return $retDiv;
		} // _processViewJson()

		function _makeListItemFromJson($list, params) {
			var p = {
				ParentId: '',
				DivId: '',
				title: '',
				json: '',
				PageType: '',
				parentlevel: '',
				level: '',
				HideRefreshButton: false,
				HideSearchButton: false,
				nextTab: '',
				suppressProps: false
			};
			if (params) $.extend(p, params);

			var id = makeSafeId({ ID: p.json['id'] });
			var text = '';

			var IsDiv = (!isNullOrEmpty(id));

			var $retLI = $('');

			switch (p.PageType) {
				case 'tab':
					{
						text = id;
						id = makeSafeId({prefix: id.replace(' ', ''), ID: p.DivId }); //we prefer nodeid_ to be at the end
						$retLI = $('<li></li>')
									.appendTo($list);
						$retLI.CswLink('init', { href: 'javascript:void(0);', value: text })
											.css('white-space', 'normal')
											.CswAttrXml({
											'data-identity': id,
											'data-url': id
										});
						
						var nextTab = (!isNullOrEmpty(p.json['value'])) ? p.json['value']['nexttab'] : '';
						if( !isNullOrEmpty(nextTab)) {
							//we're creating a tab link on a prop
							delete p.json['value']['nexttab'];
						}

						if( !p.suppressProps) {
							//we're creating a tab link which needs child props
							setTimeout(function() {
								_processViewJson({
										ParentId: p.DivId,
										DivId: id,
										title: text,
										json: p.json['value'],
										parentlevel: p.level,
										level: p.level + 1,
										PageType: 'prop',
										nextTab: nextTab,
										suppressProps: true
									});
							}, 500);
						}
						break;   
					} // case 'prop':
				case 'prop':
					{
						if( !isNullOrEmpty(p.json['value']) && !isNullOrEmpty(p.json['id']) ) {
							_FieldTypeJsonToHtml(p.json['value'], p.DivId, p.json['id'])
								.appendTo($list);
						}
						break;
					}
			}

			$retLI.unbind('click');
			$retLI.bind('click', function() {
			    return startLoadingMsg(function() {
					var par = {ParentId: p.DivId,
						parentlevel: p.parentlevel,
						level: p.parentlevel + 1,
						DivId: id,
						persistBindEvent: true,
						title: text  };
					var $div = _addPageDivToBody(par);
					par.onPageShow = function() { return _loadDivContents(par); };
					$div.CswUnbindJqmEvents();
					$div.CswBindJqmEvents(par);
					$div.CswChangePage({ reloadPage: true });
				});
			});
			
			return $retLI;
		}// _makeListItemFromJson()

		function _FieldTypeJsonToHtml(json, ParentId, IdStr) {
			/// <summary>
			///   Converts JSON into DOM content
			/// </summary>
			/// <param name="json" type="Object">A JSON Object</param>
			/// <param name="ParentId" type="String">The ElementID of the parent control (should be a prop)</param>
			/// <param name="IdStr" type="String">The ElementID of the child control</param>
			var $retLi = $('');
			if( !isNullOrEmpty(json)) {
				var FieldType = json['fieldtype'];
				var PropName = json['prop_name'];
				var ReadOnly = isTrue(json['isreadonly']);

				// Subfield values
				var sf_text = tryParseString(json['text'], '');
				var sf_value = tryParseString(json['value'], '');
				var sf_href = tryParseString(json['href'], '');
				var sf_checked = tryParseString(json['checked'], '');
				//var sf_relationship = tryParseString(json['value']['name'], '');
				var sf_required = tryParseString(json['required'], '');
				var sf_units = tryParseString(json['units'], '');
				var sf_answer = tryParseString(json['answer'], '');
				var sf_allowedanswers = tryParseString(json['allowedanswers'], '');
				var sf_correctiveaction = tryParseString(json['correctiveaction'], '');
				var sf_comments = tryParseString(json['comments'], '');
				var sf_compliantanswers = tryParseString(json['compliantanswers'], '');
				var sf_options = tryParseString(json['options'], '');

				$retLi = $('<li id="' + IdStr + '_li"></li>')
					.CswAttrXml('data-icon', false);
				var $label = $('<h2 id="' + IdStr + '_label" style="white-space:normal;" class="csw_prop_label">' + PropName + '</h2>')
					.appendTo(($retLi));

				var $fieldcontain = $('<div class="csw_fieldset" ></div>')
					.appendTo($retLi);

				var $propDiv = $('<div></div>');

				if (FieldType === "Question" &&
					!(sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) &&
						isNullOrEmpty(sf_correctiveaction)) {
					$label.addClass('OOC');
				} else {
					$label.removeClass('OOC');
				}

				var $prop;
				var propId = IdStr + '_input';

				if (!ReadOnly) {
					var addChangeHandler = true;

					switch (FieldType) {
					case "Date":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.date, ID: propId, value: sf_value });
						break;
					case "Link":
						$prop = $propDiv.CswLink('init', { ID: propId, href: sf_href, rel: 'external', value: sf_text });
						break;
					case "List":
						$prop = $('<select class="csw_prop_select" name="' + propId + '" id="' + propId + '"></select>')
							.appendTo($propDiv)
							.selectmenu();
						var selectedvalue = sf_value;
						var optionsstr = sf_options;
						var options = optionsstr.split(',');
						for (var i = 0; i < options.length; i++) {
							var $option = $('<option value="' + options[i] + '"></option>')
								.appendTo($prop);
							if (selectedvalue === options[i]) {
								$option.CswAttrDom('selected', 'selected');
							}

							if (!isNullOrEmpty(options[i])) {
								$option.val(options[i]);
							} else {
								$option.valueOf('[blank]');
							}
						}
						$prop.selectmenu('refresh');
						break;
					case "Logical":
						addChangeHandler = false; //_makeLogicalFieldSet() does this for us
						_makeLogicalFieldSet(ParentId, IdStr, sf_checked, sf_required)
							.appendTo($fieldcontain);
						break;
					case "Memo":
						$prop = $('<textarea name="' + propId + '">' + sf_text + '</textarea>')
							.appendTo($propDiv);
						break;
					case "Number":
						sf_value = tryParseNumber(sf_value, '');
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.number, ID: propId, value: sf_value });
						break;
					case "Password":
							//nada
						break;
					case "Quantity":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value })
							.append(sf_units);
						break;
					case "Question":
						addChangeHandler = false; //_makeQuestionAnswerFieldSet() does this for us
						_makeQuestionAnswerFieldSet(ParentId, IdStr, sf_allowedanswers, sf_answer, sf_compliantanswers)
							.appendTo($fieldcontain);
						var hideComments = true;
						if (!isNullOrEmpty(sf_answer) &&
							(',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') < 0 &&
								isNullOrEmpty(sf_correctiveaction)) {
							$label.addClass('OOC');
							hideComments = false;
						}

						$prop = $('<div data-role="collapsible" class="csw_collapsible" data-collapsed="' + hideComments + '"><h3>Comments</h3></div>')
							.appendTo($retLi);

						var $corAction = $('<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action">' + sf_correctiveaction + '</textarea>')
							.appendTo($prop);

						if (sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) {
							$corAction.css('display', 'none');
						}
						$corAction.unbind('change');
						$corAction.bind('change', function(eventObj) {
							var $cor = $(this);
							if ($cor.val() === '') {
								$label.addClass('OOC');
							} else {
								$label.removeClass('OOC');
							}
							onPropertyChange(ParentId, eventObj, $cor.val(), IdStr + '_cor', IdStr);
						});

						$('<textarea name="' + IdStr + '_input" id="' + IdStr + '_input" placeholder="Comments">' + sf_comments + '</textarea>')
							.appendTo($prop)
							.unbind('change')
							.bind('change', function(eventObj) {
								var $com = $(this);
								onPropertyChange(ParentId, eventObj, $com.val(), IdStr + '_com', IdStr);
							});
						break;
					case "Static":
						$propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + sf_text + '</p>'));
						break;
					case "Text":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_text });
						break;
					case "Time":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.time, ID: propId, value: sf_value });
						break;
					default:
						$propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['gestalt'] + '</p>'));
						break;
					} // switch (FieldType)

					if (addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0) {
						$prop.unbind('change').bind('change', function(eventObj) {
							var $this = $(this);
							onPropertyChange(ParentId, eventObj, $this.val(), propId, Id);
						});
					}
				} else {
					$propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['gestalt'] + '</p>'));
				}
				if ($propDiv.children().length > 0) {
					$fieldcontain.append($propDiv);
				}
			}
			return $retLi;
		}

		function _FieldTypeHtmlToJson(json, elementId, propId, value) {
			/// <summary>
			///   Converts DOM content back to JSON
			/// </summary>
			/// <param name="json" type="Object">A JSON Object</param>
			/// <param name="elementId" type="String">The id of the DOM element</param>
			/// <param name="propId" type="String">The id of the property</param>
			/// <param name="value" type="String">The stored value</param>
			
			var elementName = new CswString(elementId);
			var propName = tryParseString(makeSafeId({ ID: json['id'] }), propId);
			var fieldtype = json['fieldtype'];
			//var propname = json.name;

			// subfield nodes
			var sf_text = 'text';
			var sf_value = 'value';
			//var $sf_href = json.href;
			//var $sf_options = json.options;
			var sf_checked = 'checked';
			//var $sf_required = json.required;
			//var $sf_units = json.units;
			var sf_answer = 'answer';
			//var $sf_allowedanswers = json.allowedanswers;
			var sf_correctiveaction = 'correctiveaction';
			var sf_comments = 'comments';
			//var $sf_compliantanswers = json.compliantanswers;

			var propToUpdate = '';
			switch (fieldtype) {
				case "Date":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				case "Link":
					break;
				case "List":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				case "Logical":
					if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
						propToUpdate = sf_checked;
					}
					break;
				case "Memo":
					if (elementName.contains(propName)) {
						propToUpdate = sf_text;
					}
					break;
				case "Number":
					if (elementName.contains(propName)) propToUpdate = sf_value;
					break;
				case "Password":
					break;
				case "Quantity":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				case "Question":
					if (elementName.contains(makeSafeId({ ID: propName, suffix: 'com' }))) {
						propToUpdate = sf_comments;
					} 
					else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
						propToUpdate = sf_answer;
					} 
					else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'cor' }))) {
						propToUpdate = sf_correctiveaction;
					}
					break;
				case "Static":
					break;
				case "Text":
					if (elementName.contains(propName)) {
						propToUpdate = sf_text;
					}
					break;
				case "Time":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				default:
					break;
			}
			if (!isNullOrEmpty(propToUpdate)) {
				json[propToUpdate] = value;
				json['wasmodified'] = '1';
			}
		}// _FieldTypeHtmlToJson()

		function _makeLogicalFieldSet(ParentId, IdStr, Checked, Required) {
			var Suffix = 'ans';
			var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
										 .CswAttrDom({
										 'class': 'csw_fieldset',
										 'id': IdStr + '_fieldset'
									 })
										 .CswAttrXml({
										 'data-role': 'controlgroup',
										 'data-type': 'horizontal'
									 });
									 
			var answers = ['True', 'False'];
			if ( !isTrue(Required)) {
				answers.push = 'Null';
			}
			var inputName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unique and shared

			for (var i = 0; i < answers.length; i++) {
				var answertext = '';
				switch (answers[i]) {
				case 'Null':
					answertext = '?';
					break;
				case 'True':
					answertext = 'Yes';
					break;
				case 'False':
					answertext = 'No';
					break;
				}

				var inputId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });

				$fieldset.append('<label for="' + inputId + '">' + answertext + '</label>');
				var $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] })

				// Checked is a Tristate, so isTrue() is not useful here
				if ((Checked === 'false' && answers[i] === 'False') ||
					(Checked === 'true' && answers[i] === 'True') ||
						(Checked === '' && answers[i] === 'Null')) {
					$input.CswAttrXml({ 'checked': 'checked' });
				}
				
				$input.unbind('change');
				$input.bind('change', function(eventObj) {
					var $this = $(this);
					var thisInput = $this.val();
					onPropertyChange(ParentId, eventObj, thisInput, inputId, IdStr);
					return false;
				});
			} // for (var i = 0; i < answers.length; i++)
			return $fieldset;
		}// _makeLogicalFieldSet()

		function _makeQuestionAnswerFieldSet(ParentId, IdStr, Options, Answer, CompliantAnswers) {
			var Suffix = 'ans';
			var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
										.CswAttrDom({
										'id': IdStr + '_fieldset'
									})
										.CswAttrXml({
										'data-role': 'controlgroup',
										'data-type': 'horizontal',
										'data-theme': 'b'
									});
			var answers = Options.split(',');
			var answerName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unqiue and shared

			for (var i = 0; i < answers.length; i++) {
				var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });

				$fieldset.append('<label for="' + answerid + '" id="' + answerid + '_lab">' + answers[i] + '</label');
				var $answer = $('<input type="radio" name="' + answerName + '" id="' + answerid + '" class="csw_answer" value="' + answers[i] + '" />')
								.appendTo($fieldset);
				
				if (Answer === answers[i]) {
					 $answer.CswAttrDom('checked', 'checked');
				}
				$answer.unbind('change');
				$answer.bind('change', function(eventObj) {

					var thisAnswer = $(this).val();

					var correctiveActionId = makeSafeId({ prefix: IdStr, ID: 'cor' });
					var liSuffixId = makeSafeId({ prefix: IdStr, ID: 'label' });

					var $cor = $('#' + correctiveActionId);
					var $li = $('#' + liSuffixId);

					if ((',' + CompliantAnswers + ',').indexOf(',' + thisAnswer + ',') >= 0) {
						$cor.css('display', 'none');
						$li.removeClass('OOC');

					} else {
						$cor.css('display', '');

						if (isNullOrEmpty($cor.val())) {
							$li.addClass('OOC');
						} else {
							$li.removeClass('OOC');
						}
					}
					fixGeometry();
					onPropertyChange(ParentId, eventObj, thisAnswer, answerName, IdStr);

					return false;
				});
			} // for (var i = 0; i < answers.length; i++)
			return $fieldset;
		} // _makeQuestionAnswerFieldSet()
		
		function _addPageDivToBody(params) {
			var p = {
				ParentId: undefined,
				level: 1,
				DivId: '',       // required
				title: '',
				$toolbar: $(''),
				$content: $(''),
				HideSearchButton: false,
				HideOnlineButton: false,
				HideRefreshButton: false,
				HideHelpButton: false,
				HideBackButton: false,
				HideHeaderOnlineButton: true
			};

			if (params) {
				$.extend(p, params);
			}

			p.DivId = makeSafeId({ ID: p.DivId });

			var $pageDiv = $('#' + p.DivId);

			var firstInit = (isNullOrEmpty($pageDiv) || $pageDiv.length === 0);
			
			if (firstInit) {
				$pageDiv = $body.CswDiv('init', { ID: p.DivId })
					.CswAttrXml({
							'data-role': 'page',
							'data-url': p.DivId,
							'data-title': p.title,
							'data-rel': 'page'
						});
			}
			var headerDef = {
				buttons: {
					back: makeHeaderButtonDef(CswMobileHeaderButtons.back, p.DivId),
					search: makeHeaderButtonDef(CswMobileHeaderButtons.search, p.DivId, onSearchClick)
				},
				ID: p.DivId,
			    text: p.title,
				dataId: 'csw_header',
				dataTheme: x.Theme
			};
			var mobileHeader = new CswMobilePageHeader(headerDef, $pageDiv);

			mobileHeader.back.visible(!p.HideBackButton);
			mobileHeader.search.visible(!p.HideSearchButton);
				

			if(firstInit) {
				$pageDiv.CswDiv('init', { ID: p.DivId + '_content' })
					.CswAttrXml({ 'data-role': 'content', 'data-theme': x.Theme })
					.append(p.$content);
			}

			var footerDef = {
				buttons: {
					online: makeFooterButtonDef(CswMobileFooterButtons.online, p.DivId, onOnlineClick, mobileStorage ),
					refresh: makeFooterButtonDef(CswMobileFooterButtons.refresh, p.DivId, onRefreshClick),
					fullsite: makeFooterButtonDef(CswMobileFooterButtons.fullsite, p.DivId ),
					help: makeFooterButtonDef(CswMobileFooterButtons.help, p.DivId, onHelpClick )
				},
				ID: p.DivId,
				dataId: 'csw_footer',
				dataTheme: x.Theme
			};
			var mobileFooter = new CswMobilePageFooter(footerDef, $pageDiv);

			mobileFooter.online.visible(!p.HideOnlineButton);
			mobileFooter.refresh.visible(!p.HideRefreshButton && mobileStorage.amOnline());
			mobileFooter.help.visible(!p.HideHelpButton);
			
			if( mobileStorage.pendingChanges() ) {
				mobileFooter.online.addCssClass('pendingchanges');
			}
			else {
				mobileFooter.online.removeCssClass('pendingchanges');
			}
			
			//_bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv, mobileFooter, mobileHeader);
			return $pageDiv;

		}// _addPageDivToBody()
	
		
	
		
		function onPropertyChange(DivId, eventObj, inputVal, inputId, inputPropId) {
			var logger = new CswProfileMethod('onPropertyChange');
			var $elm = $(eventObj.target);

			var name = tryParseString(inputId, $elm.CswAttrDom('id'));
			var value = tryParseString(inputVal, eventObj.target.innerText);
	   
			var nodeId = DivId.substr(DivId.indexOf('nodeid_nodes_'),DivId.length);
			var nodeJson = mobileStorage.fetchCachedNodeJson(nodeId);
			
			// update the xml and store it
			if (!isNullOrEmpty(nodeId) && !isNullOrEmpty(nodeJson)) {
				mobileStorage.addUnsyncedChange();
				resetPendingChanges();
				
				if( !isNullOrEmpty(inputPropId) )
				{
					for (var key in nodeJson['subitems'])
					{
						var tab = nodeJson['subitems'][key];
						if (!isNullOrEmpty(tab[inputPropId])) {
							var prop = tab[inputPropId];
							_FieldTypeHtmlToJson(prop, name, inputPropId, value);
							//we're only updating one prop--don't iterate them all.
							break;
						}
					}
				}
				else { //remove else as soon as we can verify we never need to enter here
					errorHandler('Could not find a prop to update');
				}
				mobileStorage.updateStoredNodeJson(nodeId, nodeJson, '1');
			}
			mobileBgTask.reset();
			cacheLogInfo(logger);
		} // onPropertyChange()

		
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