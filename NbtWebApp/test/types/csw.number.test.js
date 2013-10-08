module('Csw.number');
test('number - returns number', function () {
    deepEqual(Csw.number(0), 0, 'should return 0');
    deepEqual(Csw.number('0'), 0, 'should return 0');
    deepEqual(isNaN(Csw.number('adfs')), true, 'should return NaN');
    deepEqual(isNaN(Csw.number(-2147483648)), true, 'int32MinVal converts to NaN');
    deepEqual(Csw.number(null, 1), 1, 'should default to 1');
    deepEqual(Csw.number([]), 0, 'apparently empty array equates to 0');
});

test('isNumber - valid numbers', function () {
    deepEqual(Csw.isNumber(0), true, '0 should be a valid number');
    deepEqual(Csw.isNumber(0.1), true, '0.1 should be a valid number');
    deepEqual(Csw.isNumber(-53.467), true, '-53.467 should be a valid number');
    deepEqual(Csw.isNumber(NaN), true, 'NaN, despite its name, is of type "number"');
});

test('isNumber - invalid numbers', function () {
    deepEqual(Csw.isNumber('0'), false, '"0" should not be a valid number');
    deepEqual(Csw.isNumber(null), false, 'null should not be a valid number');
});

test('isNumeric - valid "numbers"', function () {
    deepEqual(Csw.isNumeric('1'), true, '"1" evaluates to 1');
    deepEqual(Csw.isNumeric(true), true, 'true evaluates to 1');
});

test('isNumeric - invalid "numbers"', function () {
    deepEqual(Csw.isNumeric('asd'), false, '"asd" is not numeric');
    deepEqual(Csw.isNumeric(null), false, 'null is not numeric');
    deepEqual(Csw.isNumeric(function() {}), false, 'function is not numeric');
    deepEqual(Csw.isNumeric({}), false, 'object is not numeric');
});

test('validateFloatMinValue', function () {
    deepEqual(Csw.validateFloatMinValue(5, 0), true, '5 > 0');
    deepEqual(Csw.validateFloatMinValue(5, 5), true, 'number can be equal to the min value');
});

test('validateFloatMinValue - excluderangelimits', function () {
    deepEqual(Csw.validateFloatMinValue(5, 5, true), false, 'number must be greater than the min value');
});

test('validateFloatMaxValue', function () {
    deepEqual(Csw.validateFloatMaxValue(0.1, 5.5), true, '0.1 < 5.5');
    deepEqual(Csw.validateFloatMaxValue(5, 5), true, 'number can be equal to the max value');
});

test('validateFloatMaxValue - excluderangelimits', function () {
    deepEqual(Csw.validateFloatMaxValue(5, 5, true), false, 'number must be less than max value');
});

test('validateFloatPrecision', function () {
    deepEqual(Csw.validateFloatPrecision(1.2345, 4), true, '1.2345 has a floating-point precision of 4');
});

test('validateInteger', function () {
    deepEqual(Csw.validateInteger(5), true, '5 is a valid integer');
    deepEqual(Csw.validateInteger(5.1), false, '5.1 is not a valid integer');
});

test('validateGreaterThanZero', function () {
    deepEqual(Csw.validateGreaterThanZero(null), true, 'we do not validate null numbers');
    deepEqual(Csw.validateGreaterThanZero(0.1), true, '0.1 is valid');
    deepEqual(Csw.validateGreaterThanZero(1), true, '1 is valid');
    deepEqual(Csw.validateGreaterThanZero(0), false, '0 is invalid');
    deepEqual(Csw.validateGreaterThanZero(-1), false, '-1 is invalid (less than 0)');
});

test('validateMaxLength - string character length of number not exceeded max value', function () {
    deepEqual(Csw.validateMaxLength('300', 5), true, '0 should be a valid number');
});