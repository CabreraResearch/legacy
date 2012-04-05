/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.createMaterialWizard = Csw.nbt.createMaterialWizard ||
        Csw.nbt.register('createMaterialWizard', function (cswParent, options) {
            'use strict';

            var internal = {
                ID: 'cswCreateMaterialWizard',
                exitFunc: null, //function ($wizard) {},
                startingStep: 1,
                wizard: '',
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '',
                divStep2: ''
            };

            if (options) {
                $.extend(internal, options);
            }

            var external = cswParent.div();

            internal.wizardSteps = {
                1: 'Choose Type',
                2: 'Identity',
                3: 'Validate',
                4: 'Properties',
                5: 'Size(s)'
            };

            internal.currentStepNo = internal.startingStep;

            internal.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = internal.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    internal.wizard[button].disable();
                }
                return false;
            };

            internal.makeStepOne = (function () {
                var stepOneComplete = false;

                return function () {
                    var nextBtnEnabled = function () {
                        return false;
                    };
                    
                    internal.toggleButton(internal.buttons.prev, false);
                    internal.toggleButton(internal.buttons.cancel, true);
                    internal.toggleButton(internal.buttons.finish, false);
                    internal.toggleButton(internal.buttons.next, nextBtnEnabled());

                    if (false === stepOneComplete) {
                        
                    }
                    stepOneComplete = true;
                };
            } ());

            //Step 2: 
            internal.makeStepTwo = function() {
                

            };

            internal.handleNext = function(newStepNo) {
                internal.currentStepNo = newStepNo;
                switch (newStepNo) {
                case 2:
                    internal.makeStepTwo();
                    break;
                } // switch(newstepno)
            }; // handleNext()

            internal.handlePrevious = function (newStepNo) {
                    internal.currentStepNo = newStepNo;
                    switch (newStepNo) {
                        case 1:
                            internal.makeStepOne();
                            break;
                    }
                },

            internal.wizard = Csw.layouts.wizard(external, {
                ID: Csw.makeId(internal.ID, 'wizard'),
                Title: 'Create Material',
                StepCount: 5,
                Steps: internal.wizardSteps,
                StartingStep: internal.startingStep,
                FinishText: 'Finish',
                onNext: internal.handleNext,
                onPrevious: internal.handlePrevious,
                onCancel: null,
                onFinish: null,
                doNextOnInit: false
            });

            internal.makeStepOne();

            return external;
        });
} ());