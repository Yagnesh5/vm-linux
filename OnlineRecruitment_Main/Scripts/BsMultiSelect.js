

(
    (window, $) => {
        AddToJQueryPrototype('BsMultiSelect',
            (element, optionsObject, onDispose) => {
                let adapter = optionsObject && optionsObject.useCss
                ? new Bs4AdapterCss(optionsObject, $)
                : new Bs4AdapterJs(optionsObject, $);
                let facade = new Bs4Adapter(element, adapter, optionsObject, $);
                return new MultiSelect(element, optionsObject, onDispose, facade, window, $);
            }, $);
    }
)(window, $)
