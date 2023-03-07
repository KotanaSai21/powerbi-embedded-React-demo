// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import 'react-app-polyfill/ie11';	// For IE compatibility
import 'react-app-polyfill/stable';	// For IE compatibility
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';

ReactDOM.render(
	<App/>,
	document.getElementById('root')
);