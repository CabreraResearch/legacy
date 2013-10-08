module('Csw.array');
test('isArray - valid arrays', function () {
    ok(Csw.isArray([1, 2]), 'array of numbers is valid.');
    ok(Csw.isArray(['1', '2']), 'array of strings is valid.');
    ok(Csw.isArray([{}, {}]), 'array of objects is valid.');
    ok(Csw.isArray([[], []]), 'array of arrays is valid.');
    ok(Csw.isArray([function () { }, function () { }]), 'array of functions is valid.');
});

test('isArray - invalid arrays', function () {
    ok(false === Csw.isArray('1, 2'), 'string is not a valid array.');
    ok(false === Csw.isArray(0), 'number is not a valid array.');
    ok(false === Csw.isArray(null), 'null is not a valid array.');
    ok(false === Csw.isArray(function () { }), 'function is not a valid array.');
});

test('array - valid array', function () {
    var array = Csw.array(1, 2);
    ok(Csw.isArray(array), 'this should be a valid array:' + array);
});

test('makeSequentialArray - valid array', function () {
    var array = Csw.makeSequentialArray(1, 10);
    ok(Csw.isArray(array), 'this should be a valid array: ' + array);
    deepEqual(array.length, 10, 'this array should have 10 elements');
});