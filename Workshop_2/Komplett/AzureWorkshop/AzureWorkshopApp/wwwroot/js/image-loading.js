var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var _this = this;
var gallery = [];
var fetchImageLinks = function () {
    fetch("api/Images").then(function (response) { return __awaiter(_this, void 0, void 0, function () {
        var fetchedImageLinks, cleanedImageLinks, newImages, divContainer, _i, newImages_1, image, imageWrapper;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!!response.ok) return [3 /*break*/, 2];
                    return [4 /*yield*/, handleError(response)];
                case 1:
                    _a.sent();
                    return [2 /*return*/];
                case 2: return [4 /*yield*/, response.json()];
                case 3:
                    fetchedImageLinks = _a.sent();
                    cleanedImageLinks = fetchedImageLinks.map(function (f) { return ({
                        cleanLink: f.split("?")[0],
                        actualLink: f
                    }); });
                    newImages = findNewImages(cleanedImageLinks);
                    if (newImages.length === 0) {
                        return [2 /*return*/];
                    }
                    divContainer = document.getElementById("stored-images");
                    for (_i = 0, newImages_1 = newImages; _i < newImages_1.length; _i++) {
                        image = newImages_1[_i];
                        imageWrapper = createImageElement(image.actualLink);
                        divContainer.appendChild(imageWrapper);
                    }
                    gallery = cleanedImageLinks;
                    _a.label = 4;
                case 4: return [2 /*return*/];
            }
        });
    }); });
};
var handleError = function (response) { return __awaiter(_this, void 0, void 0, function () {
    var errorContainer, error, _a, paragraph;
    return __generator(this, function (_b) {
        switch (_b.label) {
            case 0:
                console.error(response);
                errorContainer = document.getElementById("errors");
                if (!(response.status !== 500)) return [3 /*break*/, 2];
                return [4 /*yield*/, response.json()];
            case 1:
                _a = _b.sent();
                return [3 /*break*/, 3];
            case 2:
                _a = null;
                _b.label = 3;
            case 3:
                error = _a;
                paragraph = document.createElement("p");
                paragraph.appendChild(document.createTextNode(error ? JSON.stringify(error) : response.statusText.toString()));
                errorContainer.replaceChildren(paragraph);
                ;
                return [2 /*return*/];
        }
    });
}); };
var findNewImages = function (recentList) {
    return recentList.filter(function (i) { return !gallery.map(function (g) { return g.cleanLink; }).includes(i.cleanLink); });
};
var createImageElement = function (url) {
    var imageWrapper = document.createElement("span");
    imageWrapper.className = "image-wrapper";
    var imageElement = document.createElement("img");
    imageElement.src = url;
    imageElement.className = "image";
    imageWrapper.appendChild(imageElement);
    return imageWrapper;
};
/* Dropzone */
// "imageUpload" is the camelized version of the HTML element's ID
Dropzone.options.imageUpload = {
    paramName: "files",
    dictDefaultMessage: "Drop files here or Click to Upload",
    addRemoveLinks: true,
    init: function () {
        var myDropzone = this;
        myDropzone.on("success", function (file, response) {
            myDropzone.removeFile(file);
            fetchImageLinks();
        });
    }
};
fetchImageLinks();
setInterval(function () {
    fetchImageLinks();
}, 5000);
//# sourceMappingURL=image-loading.js.map