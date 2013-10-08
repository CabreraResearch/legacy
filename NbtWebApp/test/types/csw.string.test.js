module('Csw.string');
test('string - returns string', function () {
    deepEqual(Csw.string(0), '0', '0 should evaluate to "0"');
    deepEqual(Csw.string('0'), '0', '"0" should evaluate to "0"');
    deepEqual(Csw.string(null), '', 'null should evalute to empty string');
    deepEqual(Csw.string([]), '', 'empty array should evalute to empty string');
    deepEqual(Csw.string({}), '', 'object should evalute to empty string');
    deepEqual(Csw.string([1,2,3]), '1,2,3', 'array should be string-ified');
    deepEqual(Csw.string({ str: '1' }), '', 'object should evalute to empty string');
    deepEqual(Csw.string(function() {}), '', 'function should evalute to empty string');
});

test('isString - valid strings', function () {
    deepEqual(Csw.isString(''), true, '"" is a valid string');
    deepEqual(Csw.isString('0'), true, '"0" is a valid string');
    deepEqual(Csw.isString('asdf'), true, '"asdf" is a valid string');
});

test('isString - invalid strings', function () {
    deepEqual(Csw.isString(null), false, 'null is not a string');
    deepEqual(Csw.isString([]), false, 'array is not a string');
    deepEqual(Csw.isString({}), false, 'object is not a string');
    deepEqual(Csw.isString(function () { }), false, 'function is not a string');
});

test('hrefString - returns unique string', function () {
    var hrefStr = 'htpp:\\www.google.com';
    notDeepEqual(Csw.hrefString(hrefStr), hrefStr, 'hrefStr should have a unique id appended.');
});

test('delimitedString', function () {
    var delimString = '1,2,3,4,5';
    var delimStrObj = Csw.delimitedString(delimString);
    deepEqual(delimStrObj.string('_'), '1_2_3_4_5', 'delimStrObj.string(delim) return original string with new delimiter');
    deepEqual(delimStrObj.toString(), delimString, 'delimStrObj contains original string');
    deepEqual(delimStrObj.delimited(), delimString, 'delimStrObj.delimited returns original string');
    deepEqual(delimStrObj.array, ['1', '2', '3', '4', '5'], 'delimStrObj contains string as array');
    deepEqual(delimStrObj.count(), 5, 'delimStrObj contains 5 elements');
    deepEqual(delimStrObj.contains('2'), true, '"2" exists within delimStrObj');
    deepEqual(delimStrObj.first(), '1', '"1" is the first element in delimStrObj');
    delimStrObj.add('newStr');
    deepEqual(delimStrObj.count(), 6, '"newStr" has been added to delimStrObj');
    deepEqual(delimStrObj.contains('newstr'), true, '"newstr" matches "newStr" delimStrObj');
    deepEqual(delimStrObj.contains('newstr', true), false, '"newstr" is not in delimStrObj (case sensitive)');
    delimStrObj.addToFront('string2');
    deepEqual(delimStrObj.first(), 'string2', '"string2" has been added to the front of delimStrObj');
});

test('startsWith', function () {
    deepEqual(Csw.startsWith('asdf', 'a'), true, 'asdf starts with a');
    deepEqual(Csw.startsWith('asdf', 's'), false, 'asdf does not start with s');
});