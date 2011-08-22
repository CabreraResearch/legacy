/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

var CswImportInspectionQuestions_WizardSteps = {
    'step1': { step: 1, description: 'Select File For Upload' },
    'step2': { step: 2, description: 'Upload Completed Okay' }
};

; (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswImportInspectionQuestions = function (options) 
	{
		var o = {
			ImportFileUrl: '/NbtWebApp/wsNBT.asmx/ImportInspectionQuestions',
			viewid: '',
			viewname: '',
			viewmode: '',
			ID: 'importinspectionquestions',
			onCancel: function($wizard) {},
			onFinish: function($wizard) {},
			startingStep: 1
		};
		if(options) $.extend(o, options);

		var WizardStepArray = [ CswImportInspectionQuestions_WizardSteps.step1, CswImportInspectionQuestions_WizardSteps.step2];
		var WizardSteps = {};                
		for( var i = 1; i <= WizardStepArray.length; i++ )
		{                
			WizardSteps[i] = WizardStepArray[i-1].description;
		}

		var CurrentStep = o.startingStep;

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		var $wizard = $div.CswWizard('init', { 
				'ID': o.ID + '_wizard',
				'Title': 'Import Inspection Questions',
				'StepCount': WizardStepArray.length,
				'Steps': WizardSteps,
				'StartingStep': o.startingStep,
				'FinishText': 'Save and Finish',
				'onNext': _handleNext,
				'onPrevious': _handlePrevious,
				'onBeforePrevious': _onBeforePrevious,
				'onCancel': o.onCancel,
				'onFinish': o.onFinish
			});

		// don't activate Save and Finish until step 2
		if(o.startingStep === 1)
			$wizard.CswWizard('button', 'finish', 'disable');

		// Step 1 - Select file for Upload
		var $div1 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step1.step);
        var downloadbutton = "<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Excel template</a>";
		$div1.append(downloadbutton);

        var inspectionname = "<br/><br/>Please enter the name of the new inspection: ";
        $div1.append(inspectionname);
        var $TextBox = $div1.CswInput('init',{ID: o.ID + '_inspectionname', type: CswInput_Types.text});

		var instructions1 = "<br/><br/>Please select your Excel file containing your inspection questions.<br/><br/>";
		$div1.append(instructions1);
		var $fileuploaddiv = $('<div></div>');
    
        // define parameters to pass into FileUploader
		var o = {
			url: '/NbtWebApp/wsNBT.asmx/uploadInspectionFile',
			params: 
            {
            InspectionName: _getNewInspectionName()
            },
			onSuccess: function() 
            { 
            _handleNext($wizard, CswImportInspectionQuestions_WizardSteps.step2.step);
            }
		};

		var uploader = new qq.FileUploader({
			element: $fileuploaddiv.get(0),
			action: o.url,
			params: o.params,
			debug: false,
            onSubmit: function()
            {
                o.params['InspectionName'] = _getNewInspectionName();
            },
			onComplete: function() 
				{ 
					o.onSuccess(); 
				}
		});
		$div1.append($fileuploaddiv);

		//$wizard.CswWizard('button', 'next', 'disable');

        function _getNewInspectionName()
        {
            // I am doing one basic step at a time to help with debugging
            var inspectionNameElement = $div1.find('#importinspectionquestions_inspectionname');
            var inspectionNameValue = inspectionNameElement.val();
            return (inspectionNameValue);
        }

		// Step 2 - Upload File
		var $div2 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step2.step);
		var instructions2 = "Thanks for using the Inspections Questions import wizard.<br/><br/>";
		$div2.append(instructions2);
		
		function _onBeforePrevious($wizard, stepno)
		{
			return (stepno !== CswImportInspectionQuestions_WizardSteps.step2.step || confirm("You will lose any changes made to the current view if you continue.  Are you sure?") );
		}

		function _handleNext($wizard, newstepno)
		{
			CurrentStep = newstepno;
			switch(newstepno)
			{
				case CswImportInspectionQuestions_WizardSteps.step1.step:
					break;
				case CswImportInspectionQuestions_WizardSteps.step2.step:
 					$wizard.CswWizard('button', 'finish', 'enable');
					$wizard.CswWizard('button', 'next', 'click');
					$wizard.CswWizard('button', 'next', 'disable');

					break;
			} // switch(newstepno)
		} // _handleNext()


		function _handlePrevious($wizard, newstepno)
		{
			if(newstepno === 1)
				$wizard.CswWizard('button', 'finish', 'disable');
			
			CurrentStep = newstepno;
			switch(newstepno)
			{
				case CswImportInspectionQuestions_WizardSteps.step1.step: 
					break;
				case CswImportInspectionQuestions_WizardSteps.step2.step: 
					break;
			}
		}


		function _handleFinish($wizard)
		{
		} //_handleFinish



		return $div;

	} // $.fn.CswImportInspectionQuestions_WizardSteps
}) (jQuery);

