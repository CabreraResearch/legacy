/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswEnums() {
    'use strict';


    Csw.enums.constants.register('unknownEnum', 'unknown');

    Csw.enums.register('tryParse', function (cswEnum, enumMember, caseSensitive) {
        'use strict';
        /// <summary>   Try to fetch an enum based on a string value. </summary>
        var ret = Csw.enums.constants.unknownEnum;
        if (Csw.contains(cswEnum, enumMember)) {
            ret = cswEnum[enumMember];
        } else if (false === caseSensitive) {
            Csw.each(cswEnum, function (member) {
                if (Csw.contains(cswEnum, member) &&
                    Csw.string(member).toLowerCase() === Csw.string(enumMember).toLowerCase()) {
                    ret = member;
                }
            });
        }
        return ret;
    });

    Csw.enums.register('getName', function (cswEnum, enumValue) {
        'use strict';
        /// <summary>   Try to fetch an enum name based on its value. </summary>
        var ret = '';

        Csw.each(cswEnum, function (value, name) {
            if (value === enumValue) {
                ret = name;
                return true;
            }
        });
        return ret;
    });

    Csw.enums.register('editMode', {
        View: 'View',
        Edit: 'Edit',
        Add: 'Add',
        Demo: 'Demo',
        PrintReport: 'PrintReport',
        AuditHistoryInPopup: 'AuditHistoryInPopup',
        Preview: 'Preview',
        Table: 'Table',
        Temp: 'Temp'
    });


    Csw.enums.register('errorType', {
        warning: {
            name: 'warning',
            cssclass: 'CswErrorMessage_Warning'
        },
        error: {
            name: 'error',
            cssclass: 'CswErrorMessage_Error'
        }
    });

    Csw.enums.register('ajaxUrlPrefix', 'wsNBT.asmx/');

    Csw.enums.register('events', {
        CswNodeDelete: 'CswNodeDelete',
        Submit_Request: 'Submit_Request',
        objectClassButtonClick: 'objectClassButtonClick',
        afterObjectClassButtonClick: 'afterObjectClassButtonClick',
        RestoreViewContext: 'RestoreViewContext',
        DispenseContainer: 'DispenseContainer',
        main: {
            handleAction: 'handleAction',
            refresh: 'refresh',
            refreshHeader: 'refreshHeader',
            refreshSelected: 'refreshSelected',
            clear: 'clear',
            reauthenticate: 'reauthenticate'
        }
    });

    Csw.enums.register('wizardSteps_InspectionDesign', {
        step1: { step: 1, description: 'Select an Inspection Target' },
        step2: { step: 2, description: 'Select an Inspection Design' },
        step3: { step: 3, description: 'Upload Template' },
        step4: { step: 4, description: 'Review Inspection Design' },
        step5: { step: 5, description: 'Finish' },
        stepcount: 5
    });

    Csw.enums.register('wizardSteps_FutureScheduling', {
        step1: { step: 1, description: 'Specify Schedules' },
        step2: { step: 2, description: 'Review' },
        stepcount: 2
    });

    Csw.enums.register('wizardSteps_LegacyMobile', {
        step1: { step: 1, description: 'Upload Data File' },
        step2: { step: 2, description: 'Review' },
        stepcount: 2
    });

    Csw.enums.register('dialogButtons', {
        1: 'ok',
        2: 'ok/cancel',
        3: 'yes/no'
    });

    Csw.enums.register('nbtButtonAction', {
        batchop: 'batchop',
        creatematerial: 'creatematerial',
        dispense: 'dispense',
        reauthenticate: 'reauthenticate',
        move: 'move',
        receive: 'receive',
        refresh: 'refresh',
        refreshall: 'refreshall',
        refreshonadd: 'refreshonadd',
        popup: 'popup',
        request: 'request',
        landingPage: 'landingpage',
        loadView: 'loadview',
        editprop: 'editprop',
        nothing: 'nothing',
        griddialog: 'griddialog',
        managelocations: 'Manage Locations',
        chemwatch: 'chemwatch',
        openfile: 'openfile'
    });


    Csw.enums.register('inputTypes', {
        button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        'datetime-local': { id: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ },
        text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px', defaultsize: '25'/*characters*/ },
        week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '', defaultsize: '25'/*characters*/ }
    });

    Csw.enums.register('viewMode', {
        grid: { name: 'Grid' },
        tree: { name: 'Tree' },
        list: { name: 'List' },
        table: { name: 'Table' }
    });

    Csw.enums.register('rateIntervalTypes', {
        Hourly: 'Hourly',
        WeeklyByDay: 'WeeklyByDay',
        MonthlyByDate: 'MonthlyByDate',
        MonthlyByWeekAndDay: 'MonthlyByWeekAndDay',
        YearlyByDate: 'YearlyByDate'
    });

    Csw.enums.register('imageButton_ButtonType', {
        None: -1,
        Add: 27,
        ArrowNorth: 28,
        ArrowEast: 29,
        ArrowSouth: 30,
        ArrowWest: 31,
        Calendar: 6,
        CheckboxFalse: 18,
        CheckboxNull: 19,
        CheckboxTrue: 20,
        Clear: 4,
        Clock: 10,
        ClockGrey: 11,
        Configure: 26,
        Delete: 4,
        Edit: 3,
        Fire: 5,
        PageFirst: 23,
        PagePrevious: 24,
        PageNext: 25,
        PageLast: 22,
        PinActive: 17,
        PinInactive: 15,
        Print: 2,
        Refresh: 9,
        SaveStatus: 13,
        Select: 32,
        TableSingleColumn: 33,
        TableMultiColumn: 34,
        ToggleActive: 1,
        ToggleInactive: 0,
        View: 8
    });

    Csw.enums.register('iconState', {
        none: -1,
        normal: 0,
        hover: 1,
        selected: 2,
        disabled: 3
    });


    Csw.enums.register('iconType', {
        none: -1,
        about: 0,
        info: 1,
        plus: 2,
        add: 2,
        minus: 3,
        up: 4,
        right: 5,
        down: 6,
        left: 7,
        back: 7,
        play: 8,
        fastforward: 9,
        rewind: 10,
        undo: 11,
        redo: 12,
        refresh: 13,
        check: 14,
        x: 15,
        stop: 16,
        cancel: 17,
        xbutton: 18,
        rightbutton: 19,
        explode: 20,
        star: 21,
        staradd: 22,
        docrefresh: 23,
        docimport: 24,
        docexport: 25,
        person: 26,
        people: 27,
        contact: 28,
        calendar: 29,
        calculator: 30,
        phone: 31,
        world: 32,
        cart: 33,
        flag: 34,
        copy: 35,
        barchart: 36,
        harddrive: 37,
        bell: 38,
        questionmark: 39,
        trash: 40,
        scissors: 41,
        lock: 42,
        unlock: 43,
        envelope: 44,
        toolbox: 45,
        wrench: 46,
        doc: 47,
        magglass: 48,
        magplus: 49,
        magminus: 50,
        pencil: 51,
        folder: 52,
        foldergear: 53,
        gear: 54,
        gearset: 55,
        trianglegear: 56,
        clipboardgear: 57,
        clipboardcheck: 58,
        target: 59,
        targetgroup: 60,
        compass: 61,
        barcode: 62,
        dna: 63,
        atom: 64,
        flask: 65,
        house: 66,
        door: 67,
        cabinet: 68,
        box: 69,
        warning: 70,
        save: 71,
        search: 72,
        cat: 73,
        atommag: 74,
        atomplus: 75,
        cartplus: 76,
        bottle: 77,
        bottlebox: 78,
        bottlecalendar: 79,
        calendarstack: 80,
        envelopes: 81,
        personclock: 82,
        starsolid: 83,
        sequence: 84
    });


    Csw.enums.register('searchCssClasses', {
        nodetype_select: { name: 'csw_search_nodetype_select' },
        property_select: { name: 'csw_search_property_select' }
    });


    Csw.enums.register('appMode', {
        mode: 'full'
    });


    Csw.enums.register('wizardSteps_ViewEditor', {
        viewselect: { step: 1, description: 'Choose a View', divId: 'step1_viewselect' },
        attributes: { step: 2, description: 'Edit View Attributes', divId: 'step2_attributes' },
        relationships: { step: 3, description: 'Add Relationships', divId: 'step3_relationships' },
        properties: { step: 4, description: 'Select Properties', divId: 'step4_properties' },
        filters: { step: 5, description: 'Set Filters', divId: 'step5_filters' },
        tuning: { step: 6, description: 'Fine Tuning', divId: 'step6_tuning' }
    });


    Csw.enums.register('cssClasses_ViewBuilder', {
        conjunction_select: { name: 'csw_viewbuilder_conjunction_select' },
        subfield_select: { name: 'csw_viewbuilder_subfield_select' },
        filter_select: { name: 'csw_viewbuilder_filter_select' },
        default_filter: { name: 'csw_viewbuilder_default_filter' },
        filter_value: { name: 'csw_viewbuilder_filter_value' },
        metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
    });

    Csw.enums.register('domElementEvent', {
        click: { name: 'click' },
        change: { name: 'change' },
        vclick: { name: 'vclick' },
        tap: { name: 'tap' }
    });

    Csw.enums.register('objectClasses', {
        GenericClass: 'GenericClass',
        InspectionDesignClass: 'InspectionDesignClass'
    });

    Csw.enums.register('nodeSpecies', {
        Plain: 'Plain',
        More: 'More'
    });

    Csw.enums.register('subFieldNames', {
        Unknown: { name: 'unknown' },
        AllowedAnswers: { name: 'allowedanswers' },
        Answer: { name: 'answer' },
        Barcode: { name: 'barcode' },
        Blob: { name: 'blob' },
        Checked: { name: 'checked' },
        Column: { name: 'column' },
        Comments: { name: 'comments' },
        CompliantAnswers: { name: 'compliantanswers' },
        ContentType: { name: 'contenttype' },
        CorrectiveAction: { name: 'correctiveaction' },
        DateAnswered: { name: 'dateanswered' },
        DateCorrected: { name: 'datecorrected' },
        Href: { name: 'href' },
        Image: { name: 'image' },
        Interval: { name: 'interval' },
        IsCompliant: { name: 'iscompliant' },
        Mol: { name: 'mol' },
        Name: { name: 'name' },
        NodeID: { name: 'nodeid' },
        NodeType: { name: 'nodetype' },
        Number: { name: 'number' },
        Password: { name: 'password' },
        Path: { name: 'path' },
        Required: { name: 'required' },
        Row: { name: 'row' },
        Sequence: { name: 'sequence' },
        StartDateTime: { name: 'startdatetime' },
        Text: { name: 'text' },
        Units: { name: 'units' },
        Value: { name: 'value' },
        ViewID: { name: 'viewid' },
        ChangedDate: { name: 'changeddate' },
        Base: { name: 'base' },
        Exponent: { name: 'exponent' }
    });


    Csw.enums.register('subFieldsMap', {
        AuditHistoryGrid: { name: 'AuditHistoryGrid', subfields: {} },
        Barcode: {
            name: 'Barcode',
            subfields: {
                Barcode: Csw.enums.subFieldNames.Barcode,
                Sequence: Csw.enums.subFieldNames.Number
            }
        },
        Button: {
            name: 'Button',
            subfields: {
                Text: Csw.enums.subFieldNames.Text
            }
        },
        CASNo: {
            name: 'CASNo',
            subfields: {
                Text: Csw.enums.subFieldNames.Text
            }
        },
        ChildContents: { name: 'ChildContents', subfields: {} },
        Comments: { name: 'Comments', subfields: {} },
        Composite: { name: 'Composite', subfields: {} },
        DateTime: {
            name: 'DateTime',
            subfields: {
                Value: {
                    Date: { name: 'date' },
                    Time: { name: 'time' },
                    DateFormat: { name: 'dateformat' },
                    TimeFormat: { name: 'timeformat' }
                },
                DisplayMode: {
                    Date: { name: 'Date' },
                    Time: { name: 'Time' },
                    DateTime: { name: 'DateTime' }
                }
            }
        },
        File: { name: 'File', subfields: {} },
            Formula: { name: 'Formula', subfields: {} },
        Grid: { name: 'Grid', subfields: {} },
        Image: { name: 'Image', subfields: {} },
        ImageList: { name: 'ImageList', subfields: {} },
        Link: {
            name: 'Link',
            subfields: {
                Text: Csw.enums.subFieldNames.Text,
                Href: Csw.enums.subFieldNames.Href
            }
        },
        List: {
            name: 'List',
            subfields: {
                Value: Csw.enums.subFieldNames.Value
            }
        },
        Location: { name: 'Location', subfields: {} },
        LocationContents: { name: 'LocationContents', subfields: {} },
        Logical: {
            name: 'Logical',
            subfields: {
                Checked: Csw.enums.subFieldNames.Checked
            }
        },
        LogicalSet: { name: 'LogicalSet', subfields: {} },
            MetaDataList: { name: 'MetaDataList', subfields: {} },
        Memo: {
            name: 'Memo',
            subfields: {
                Text: Csw.enums.subFieldNames.Text
            }
        },
        MOL: {
            name: 'MOL',
            subfields: {

            }
        },
        MTBF: { name: 'MTBF', subfields: {} },
        MultiList: { name: 'MultiList', subfields: {} },
        NFPA: { name: 'NFPA', subfields: {} },
        NodeTypeSelect: { name: 'NodeTypeSelect', subfields: {} },
        Number: {
            name: 'Number',
            subfields: {
                Value: Csw.enums.subFieldNames.Value
            }
        },
        NumericRange: {
            name: 'NumericRange',
            subfields: {
                Lower: Csw.enums.subFieldNames.Lower,
                Target: Csw.enums.subFieldNames.Target,
                Upper: Csw.enums.subFieldNames.Upper,
                LowerInclusive: Csw.enums.subFieldNames.LowerInclusive,
                UpperInclusive: Csw.enums.subFieldNames.UpperInclusive
            }
        },
        Password: {
            name: 'Password',
            subfields: {
                Password: Csw.enums.subFieldNames.Password,
                ChangedDate: Csw.enums.subFieldNames.ChangedDate
            }
        },
        PropertyReference: { name: 'PropertyReference', subfields: {} },
        Quantity: {
            name: 'Quantity',
            subfields: {
                Value: Csw.enums.subFieldNames.Value,
                Units: Csw.enums.subFieldNames.Number
            }
        },
        Question: {
            name: 'Question',
            subfields: {
                Answer: Csw.enums.subFieldNames.Answer,
                CorrectiveAction: Csw.enums.subFieldNames.CorrectiveAction,
                IsCompliant: Csw.enums.subFieldNames.IsCompliant,
                Comments: Csw.enums.subFieldNames.Comments,
                DateAnswered: Csw.enums.subFieldNames.DateAnswered,
                DateCorrected: Csw.enums.subFieldNames.DateCorrected
            }
        },
        Relationship: { name: 'Relationship', subfields: {} },
        ReportLink: { name: 'ReportLink', subfields: {} },
        Scientific: { name: 'Scientific', subfields: {} },
        Sequence: { name: 'Sequence', subfields: {} },
        Static: {
            name: 'Static',
            subfields: {
                Text: Csw.enums.subFieldNames.Text
            }
        },
        Text: {
            name: 'Text',
            subfields: {
                Text: Csw.enums.subFieldNames.Text
            }
        },
        TimeInterval: { name: 'TimeInterval', subfields: {} },
        UserSelect: { name: 'UserSelect', subfields: {} },
        ViewPickList: { name: 'ViewPickList', subfields: {} },
        ViewReference: { name: 'ViewReference', subfields: {} }
    });

    Csw.enums.register('cssClasses_ViewEdit', {
        vieweditor_viewrootlink: { name: 'vieweditor_viewrootlink' },
        vieweditor_viewrellink: { name: 'vieweditor_viewrellink' },
        vieweditor_viewproplink: { name: 'vieweditor_viewproplink' },
        vieweditor_viewfilterlink: { name: 'vieweditor_viewfilterlink' },
        vieweditor_addfilter: { name: 'vieweditor_addfilter' },
        vieweditor_deletespan: { name: 'vieweditor_deletespan' },
        vieweditor_childselect: { name: 'vieweditor_childselect' }
    });

    Csw.enums.register('viewChildPropNames', {
        root: { name: 'root' },
        childrelationships: { name: 'childrelationships' },
        properties: { name: 'properties' },
        filters: { name: 'filters' },
        propfilters: { name: 'filters' },
        filtermodes: { name: 'filtermodes' }
    });

    Csw.enums.register('nodeTree_DefaultSelect', {
        root: { name: 'root' },
        firstchild: { name: 'firstchild' },
        none: { name: 'none' }
    });

    Csw.enums.register('toggleState', {
        on: { name: 'on' },
        off: { name: 'off' }
    });

    Csw.enums.register('NFPADisplayMode', {
        Linear: 'Linear',
        Diamond: 'Diamond'
    });

}());
