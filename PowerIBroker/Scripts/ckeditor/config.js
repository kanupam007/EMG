/// <reference path="styles.js" />
/// <reference path="styles.js" />
/**
 * Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
    // config.language = 'fr';
    
    //config.uiColor = '#AADC6E';
    config.bodyClass = 'content';
    config.contentsCss = '/Scripts/ckeditor/styles.js';
    config.font_names = 'Open_Sansregular;Helvetica Neue; Helvetica,sans-serif;' + CKEDITOR.config.font_names;
    
};

