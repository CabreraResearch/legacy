//#region Global

var EditMode = {
	Edit: { name: 'Edit' },
	AddInPopup: { name: 'AddInPopup' },
	EditInPopup: { name: 'EditInPopup' },
	Demo: { name: 'Demo' },
	PrintReport: { name: 'PrintReport' },
	DefaultValue: { name: 'DefaultValue' },
	AuditHistoryInPopup: { name: 'AuditHistoryInPopup' }
};

// for CswInput
var CswInput_Types = {
	button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
	checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
	color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
	date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
	'datetime-local': { id: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false }, defaultwidth: '' },
	hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
	image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
	month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
	number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
	password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
	range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
	reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
	search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
	submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
	tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
	text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
	week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' }
};

var CswAppMode = {
	mode: 'full'     
};

// For CswImageButton
var CswImageButton_ButtonType = {
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
	ToggleActive: 1,
	ToggleInactive: 0,
	View: 8
};

// for CswSearch
var CswSearch_CssClasses = {
	nodetype_select: { name: 'csw_search_nodetype_select' },
	property_select: { name: 'csw_search_property_select' }
};

// for CswViewEditor
var CswViewEditor_WizardSteps = {
	'step1': { step: 1, description: 'Choose a View' },
	'step2': { step: 2, description: 'Edit View Attributes' },
	'step3': { step: 3, description: 'Add Relationships' },
	'step4': { step: 4, description: 'Select Properties' },
	'step5': { step: 5, description: 'Set Filters' },
	'step6': { step: 6, description: 'Fine Tuning' }
};

// for CswViewPropFilter
var ViewBuilder_CssClasses = {
	subfield_select: { name: 'csw_viewbuilder_subfield_select' },
	filter_select: { name: 'csw_viewbuilder_filter_select' },
	default_filter: { name: 'csw_viewbuilder_default_filter' },
	filter_value: { name: 'csw_viewbuilder_filter_value' },
	metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
};

var CswDomElementEvent = {
	click: {name: 'click'},
	change: {name: 'change'},
	vclick: {name: 'vclick'},
	tap: {name: 'tap'}
};

//#endregion Global

//#region Mobile

var CswMobileHeaderButtons = {
    back: {name: 'back', 
            ID: 'back',
	        text: 'Back',
	        cssClass: 'ui-btn-left',
	        dataDir: 'reverse',
	        dataIcon: 'arrow-l',
            dataRelationship: 'back'
    },
    search: {name: 'search',
             ID: 'search',
			 text: 'Search',
			 cssClass: 'ui-btn-right'
    }
};
var CswMobileFooterButtons = {
    online: {name: 'online',
             ID: 'online',
             text: 'Online',
			 cssClass: 'ui-btn-active onlineStatus ',
			 dataIcon: 'gear'
    },
    refresh: {name: 'refresh',
              ID: 'refresh',
			  text: 'Refresh',
		      cssClass: 'refresh',
			  dataIcon: 'refresh'
    
    },
    fullsite: {name: 'main',
               ID: 'main',
	           text: 'Full Site',
	           href: 'Main.html',
	           rel: 'external',
	           dataIcon: 'home'
    },
    help: {name: 'help',
           ID: 'help',
		   text: 'Help',
		   dataIcon: 'info'
    }
};
var CswMobilePage_Type = {
    login: { name: 'login' },
    help: { name: 'help' },
    nodes: { name: 'nodes' },
    offline: {name: 'offline' },
    online: { name: 'online' },
    props: { name: 'props' },
    search: { name: 'search' },
    tabs: { name: 'tabs' },
    views: { name: 'views' }
};
var CswMobileGlobal_Config = {
    theme: 'b'
};
//#endregion Mobile