/// <reference path="SampleCode.js"/>
module('test math');
test('Sum', function () {
    var sum = add(2, 3);
    equals(5, sum, 'Sum of 2 and 3 is 5');
});

test('Difference', function () {
    var difference = subtract(5, 1);
    equals(4, difference, 'Subtracting 1 from 5 gives 4');
});

test('Product', function () {
    var product = multiply(6, 3);
    equals(18, product, 'Multiplying 6 and 3 gives 18');
});