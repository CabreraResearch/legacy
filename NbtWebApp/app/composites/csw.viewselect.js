/*global Csw,Ext,$,window */


(function () {
    "use strict";

    //Case 30479: gods help you if you need more than viewSelect in the same page at the same time.
    //until that day, let's self-satisfy our promises.
    var promise = null;
    var promise2 = null;   // kludge around the above for CIS-52469

    Csw.composites.register('viewSelect', function (cswParent, params) {

        var cswPrivate = {
            viewMethod: 'Views/ViewSelect',
            name: 'viewselect',
            onSelect: null,
            onSuccess: null,
            //ClickDelay: 300,
            issearchable: false,
            includeRecent: true,
            includeReports: true,
            includeSearches: true,
            includeActions: true,
            //usesession: true,
            hidethreshold: 5,
            maxHeight: '',
            fieldSets: {

            },
            useCache: true,
            useSecondaryPromise: false,
            div: null
        };

        Csw.extend(cswPrivate, params);

        var cswPublic = {};

        cswPrivate.addCategory = function (catobj) {
            cswPrivate.fieldSets[catobj.category + '_fs'] = cswPrivate.fieldSets[catobj.category + '_fs'] ||
                cswPrivate.vsdiv.fieldSet({ cssclass: 'viewselectfieldset' });


            cswPrivate.fieldSets[catobj.category + '_fs'].empty();
            cswPrivate.fieldSets[catobj.category + '_fs'].legend({ cssclass: 'viewselectlegend', value: catobj.category });

            var morediv = cswPrivate.fieldSets[catobj.category + '_fs'].moreDiv({
                name: 'morediv'
            });

            var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
            var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
            var row = 1;
            var tbl = showntbl;
            morediv.moreLink.hide();

            Csw.iterate(catobj.items, function (itemobj, itemname) {
                if (row > cswPrivate.hidethreshold && tbl === showntbl) {
                    row = 1;
                    tbl = hiddentbl;
                    morediv.moreLink.show();
                }

                var iconcell = tbl.cell(row, 1).addClass('viewselecticoncell');
                iconcell.img({ src: itemobj.iconurl });

                var linkcell = tbl.cell(row, 2).addClass('viewselectitemcell');
                linkcell.text(itemobj.name);

                iconcell.bind('click', function () { cswPrivate.handleSelect(itemobj); });
                linkcell.bind('click', function () { cswPrivate.handleSelect(itemobj); });

                linkcell.$.hover(
                    function () {
                        iconcell.addClass('viewselectitemhover');
                        linkcell.addClass('viewselectitemhover');
                    },
                    function () {
                        iconcell.removeClass('viewselectitemhover');
                        linkcell.removeClass('viewselectitemhover');
                    });

                row += 1;
            }); // Csw.each() items
        }; // addCategory()

        cswPrivate.handleSelect = function (itemobj) {

            var $newTopContent = $('<div></div>');
            var table = Csw.literals.table({
                $parent: $newTopContent,
                name: cswPrivate.name + '_selectedtbl'
            });
            var iconDiv = table.cell(1, 1).div();

            iconDiv.css('background-image', itemobj.iconurl);
            iconDiv.css('width', '16px');
            iconDiv.css('height', '16px');

            table.cell(1, 2).text(Csw.string(itemobj.name).substr(0, 30));

            cswPrivate.comboBox.topContent($newTopContent);
            cswPrivate.div.data('selectedType', itemobj.type);
            cswPrivate.div.data('selectedName', itemobj.name);
            cswPrivate.div.data('selectedValue', itemobj.itemid);

            Csw.tryExec(cswPrivate.onSelect, itemobj);
        }; // cswPrivate.handleSelect()

        var toDo = [];

        // Constructor
        (function ctor() {
            toDo.push(ctor);

            var getAjaxPromise = function () {
                var thisPromise = promise;
                if (cswPrivate.useSecondaryPromise) {
                    thisPromise = promise2;
                }

                if (thisPromise && thisPromise.abort) {
                    thisPromise.abort();
                }
                thisPromise = Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.viewMethod,
                    watchGlobal: false,
                    data: {
                        IsSearchable: cswPrivate.issearchable,
                        IncludeRecent: cswPrivate.includeRecent,
                        IncludeReports: cswPrivate.includeReports,
                        IncludeSearches: cswPrivate.includeSearches,
                        IncludeActions: cswPrivate.includeActions
                    },
                    success: function (ret) {
                        if (false === Csw.compare(ret, cswPrivate.data)) {
                            makeSelect(ret);
                            Csw.setCachedWebServiceCall('Services/' + cswPrivate.viewMethod, ret);
                            return Csw.tryExec(cswPrivate.onSuccess);
                        }
                    }
                });

                if (cswPrivate.useSecondaryPromise) {
                    promise2 = thisPromise;
                } else {
                    promise = thisPromise;
                }
                return thisPromise;
            };

            var makeSelect = function (data) {
                if (data) {
                    cswParent.empty();
                    cswPrivate.div = cswParent.div();
                    cswPublic = Csw.dom({}, cswPrivate.div);

                    cswPrivate.vsdiv = Csw.literals.div();
                    if (false == Csw.isNullOrEmpty(cswPrivate.maxHeight)) {
                        cswPrivate.vsdiv.css({ maxHeight: cswPrivate.maxHeight });
                    }
                    cswPrivate.comboBox = cswPrivate.div.comboBox({
                        name: cswPrivate.name + '_combo',
                        topContent: 'Select a View',
                        selectContent: cswPrivate.vsdiv.$,
                        width: '266px'
                    });

                    Csw.extend(cswPublic, cswPrivate.comboBox);

                    Csw.iterate(data.categories, cswPrivate.addCategory);
                }
                return data;
            };

            if (true === cswPrivate.useCache) {
                Csw.getCachedWebServiceCall('Services/' + cswPrivate.viewMethod)
                    .then(makeSelect)
                    .fin(function (data) {
                        cswPrivate.useCache = false;
                        return cswParent.viewSelect({
                            onSelect: cswPrivate.onSelect,
                            onSuccess: cswPrivate.onSuccess,
                            issearchable: cswPrivate.issearchable,
                            includeRecent: cswPrivate.includeRecent,
                            includeReports: cswPrivate.includeReports,
                            includeSearches: cswPrivate.includeSearches,
                            includeActions: cswPrivate.includeActions,
                            hidethreshold: cswPrivate.hidethreshold,
                            maxHeight: cswPrivate.maxHeight,
                            useCache: false,
                            data: data
                        });
                    });
            } else {
                getAjaxPromise();
            }
            //getAjaxPromise();

            var thisPromise = promise;
            if (cswPrivate.useSecondaryPromise) {
                thisPromise = promise2;
            }
            toDo.push(thisPromise);
            return thisPromise;
        })();


        cswPublic.value = function () {
            return {
                type: cswPrivate.div.data('selectedType'),
                value: cswPrivate.div.data('selectedValue'),
                name: cswPrivate.div.data('selectedName')
            };
        };

        cswPublic.hide = function () {
            cswPrivate.comboBox.hide();
        };

        cswPublic.show = function () {
            cswPrivate.comboBox.show();
        };

        cswPublic.val = cswPublic.value;

        cswPublic.promise = Q.all(toDo);

        return cswPublic;
    });
})();
