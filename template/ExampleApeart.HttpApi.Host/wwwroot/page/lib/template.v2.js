/*!
 * template.js v0.7.1 (https://github.com/yanhaijing/template.js)
 * API https://github.com/yanhaijing/template.js/blob/master/doc/api.md
 * Copyright 2015 yanhaijing. All Rights Reserved
 * Licensed under MIT (https://github.com/yanhaijing/template.js/blob/master/MIT-LICENSE.txt)
 */
;(function(root, factory) {
    const template = factory(root);
    if (typeof define === 'function' && define.amd) {
        // AMD
        define('template', function() {
            return template;
        });
    } else if (typeof exports === 'object') {
        // Node.js
        module.exports = template;
    } else {
        // Browser globals
        const _template = root.template;

        template.noConflict = function() {
            if (root.template === template) {
                root.template = _template;
            }
            return template;
        };
        root.template = template;
    }
}(this, function(root) {
    'use strict';

    // 默认配置
    const defaultOptions = {
        sTag: '<%',
        eTag: '%>',
        compress: true,
        escape: true,
        error: function (e) {}
    };

    // 内部函数和修饰器
    const internalFunctions = {
        functionMap: {},
        modifierMap: {
            '': function (param) { return param; },
            'h': function (param) { return encodeHTML(param); },
            'u': function (param) { return encodeURI(param); }
        }
    };

    // 工具函数
    function type(x) {
        if(x === null){
            return 'null';
        }
        const t = typeof x;
        if(t !== 'object'){
            return t;
        }
        const c = Object.prototype.toString.call(x).slice(8, -1).toLowerCase();
        if(c !== 'object'){
            return c;
        }
        if(x.constructor === Object){
            return c;
        }
        return 'unknown';
    }

    function isObject(obj) {
        return type(obj) === 'object';
    }

    function isFunction(fn) {
        return type(fn) === 'function';
    }

    function isString(str) {
        return type(str) === 'string';
    }

    function extend(target, ...sources) {
        sources.forEach(source => {
            for (const key in source) {
                target[key] = source[key];
            }
        });
        return target;
    }

    function clone(...args) {
        return extend({}, ...args);
    }

    function encodeHTML(source) {
        return String(source)
            .replace(/&/g,'&amp;')
            .replace(/</g,'&lt;')
            .replace(/>/g,'&gt;')
            .replace(/\\/g,'&#92;')
            .replace(/"/g,'&quot;')
            .replace(/'/g,'&#39;');
    }

    function compress(html) {
        return html.replace(/\s+/g, ' ').replace(/<!--[\w\W]*?-->/g, '');
    }

    function consoleAdapter(cmd, msg) {
        typeof console !== 'undefined' && console[cmd] && console[cmd](msg);
    }

    function handelError(e) {
        let message = 'template.js error\n\n';
        for (const key in e) {
            message += `<${key}>\n${e[key]}\n\n`;
        }
        message += `<message>\n${e.message}\n\n`;
        consoleAdapter('error', message);
        defaultOptions.error(e);
        function error() {
            return 'template.js error';
        }
        error.toString = function () {
            return '__code__ = "template.js error"';
        };
        return error;
    }

    // 解析模板
    function parse(tpl, opt) {
        const sTag = opt.sTag;
        const eTag = opt.eTag;
        const escape = opt.escape;

        function parsehtml(line) {
            line = line.replace(/('|")/g, '\\$1');
            const lineList = line.split('\n');
            let code = '';
            for (let i = 0; i < lineList.length; i++) {
                code += `;__code__ += ("${lineList[i]}${i === lineList.length - 1 ? '")\n' : '\\n")\n'}`;
            }
            return code;
        }

        function parsejs(line) {
            const reg = /^(?:=|(:.*?)=)(.*)$/;
            let html;
            let arr;
            let modifier;

            if (arr = reg.exec(line)) {
                html = arr[2];
                if (Boolean(arr[1])) {
                    modifier = arr[1].slice(1);
                } else {
                    modifier = escape ? 'h' : '';
                }
                return `;__code__ += internalFunctions.modifierMap["${modifier}"](typeof (${html}) !== "undefined" ? (${html}) : "")\n`;
            }
            return `;${line}\n`;
        }

        let code = '';
        const tokens = tpl.split(sTag);
        for (let i = 0, len = tokens.length; i < len; i++) {
            const token = tokens[i].split(eTag);
            if (token.length === 1) {
                code += parsehtml(token[0]);
            } else {
                code += parsejs(token[0], true);
                if (token[1]) {
                    code += parsehtml(token[1]);
                }
            }
        }
        return code;
    }

    // 编译模板
    function compiler(tpl, opt) {
        const mainCode = parse(tpl, opt);
        const headerCode = `
    var html = (function (__data__, __modifierMap__) {
        var __str__ = "", __code__ = "";
        for(var key in __data__) {
            __str__+=("var " + key + "=__data__['" + key + "'];");
        }
        eval(__str__);
`;
        const footerCode = `
        ;return __code__;
    }(__data__, __modifierMap__));
    return html;
`;
        const code = headerCode + mainCode + footerCode;
        const safeCode = code.replace(/[\r]/g, ' '); 
        try {
            const Render = new Function('__data__', '__modifierMap__', safeCode);
            Render.toString = function () {
                return mainCode;
            };
            return Render;
        } catch(e) {
            e.temp = `function anonymous(__data__, __modifierMap__) {${code}}`;
            throw e;
        }
    }

    function compile(tpl, opt) {
        const options = clone(defaultOptions, opt);
        try {
            const Render = compiler(tpl, options);
        } catch(e) {
            e.name = 'CompileError';
            e.tpl = tpl;
            e.render = e.temp;
            delete e.temp;
            return handelError(e);
        }

        function render(data) {
            const mergedData = clone(internalFunctions.functionMap, data);
            try {
                let html = Render(mergedData, internalFunctions.modifierMap);
                html = options.compress ? compress(html) : html;
                return html;
            } catch(e) {
                e.name = 'RenderError';
                e.tpl = tpl;
                e.render = Render.toString();
                return handelError(e)();
            }
        }

        render.toString = function () {
            return Render.toString();
        };
        return render;
    }

    function template(tpl, data) {
        if (typeof tpl !== 'string') {
            return '';
        }
        const fn = compile(tpl);
        if (!isObject(data)) {
            return fn;
        }
        return fn(data);
    }

    template.config = function (option) {
        if (isObject(option)) {
            extend(defaultOptions, option);
        }
        return clone(defaultOptions);
    };

    template.registerFunction = function(name, fn) {
        if (!isString(name)) {
            return clone(internalFunctions.functionMap);
        }
        if (!isFunction(fn)) {
            return internalFunctions.functionMap[name];
        }
        return internalFunctions.functionMap[name] = fn;
    };

    template.unregisterFunction = function (name) {
        if (!isString(name)) {
            return false;
        }
        delete internalFunctions.functionMap[name];
        return true;
    };

    template.registerModifier = function(name, fn) {
        if (!isString(name)) {
            return clone(internalFunctions.modifierMap);
        }
        if (!isFunction(fn)) {
            return internalFunctions.modifierMap[name];
        }
        return internalFunctions.modifierMap[name] = fn;
    };

    template.unregisterModifier = function (name) {
        if (!isString(name)) {
            return false;
        }
        delete internalFunctions.modifierMap[name];
        return true;
    };

    template.__encodeHTML = encodeHTML;
    template.__compress = compress;
    template.__handelError = handelError;
    template.__compile = compile;
    template.version = '0.7.1';
    return template;
}));