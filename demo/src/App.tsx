import React, { useState } from 'react';
import { models, Report, Embed, service } from 'powerbi-client';
import { PowerBIEmbed } from 'powerbi-client-react';
import 'powerbi-report-authoring';
import './App.css';

// Root Component to demonstrate usage of embedded component
function DemoApp (): JSX.Element {

	const sampleReportUrl = '<Function URL>';

	// Track the report object
	const [report, setReport] = useState<Report | null>(null);

	// Track Report embedding status
	const [isEmbedded, setIsEmbedded] = useState<boolean>(false);

    // Overall status message of embedding
	const [displayMessage, setMessage] = useState(`The report is bootstrapped. Click the Embed Report button to set the access token`);

	// CSS Class to be passed to the embedded component
	const reportClass = 'report-container';

	// Pass the basic embed configurations to the embedded component to bootstrap the report on first load
    // Values for properties like embedUrl, accessToken and settings will be set on click of button
	const [sampleReportConfig, setReportConfig] = useState<models.IReportEmbedConfiguration>({
		type: 'report',
		embedUrl: undefined,
		tokenType: models.TokenType.Embed,
		accessToken: undefined,
		settings: undefined,
	});

	const eventHandlersMap: Map<string, (event?: service.ICustomEvent<any>, embeddedEntity?: Embed) => void | null> = new Map([
		['loaded', () => console.log('Report has loaded')],
		['rendered', () => console.log('Report has rendered')],
		['error', (event?: service.ICustomEvent<any>) => {
				if (event) {
					console.error(event.detail);
				}
			},
		],
		['visualClicked', () => console.log('visual clicked')],
		['pageChanged', (event) => console.log(event)],
	]);

	const embedReport = async (): Promise<void> => {
		console.log('Embed Report clicked');

		// Get the embed config from the service
		const reportConfigResponse = await fetch(sampleReportUrl);

		if (reportConfigResponse === null) {
			return;
		}

		if (!reportConfigResponse?.ok) {
			console.error(`Failed to fetch config for report. Status: ${ reportConfigResponse.status } ${ reportConfigResponse.statusText }`);
			return;
		}

		const reportConfig = await reportConfigResponse.json();

		// Update the reportConfig to embed the PowerBI report
		setReportConfig({
			...sampleReportConfig,
			embedUrl: reportConfig.EmbedUrl,
			accessToken: reportConfig.EmbedToken
		});
		setIsEmbedded(true);

		// Update the display message
		setMessage('Report Embedded');
	};

	const controlButtons =
		isEmbedded ?
		<>
			<label className = "display-message">
				{ displayMessage }
			</label>
		</>
		:
		<>
			<label className = "display-message position">
				{ displayMessage }
			</label>

			<button onClick = { embedReport } className = "embed-report">
				Embed Report</button>
		</>;

	const reportComponent =
		<PowerBIEmbed
			embedConfig = { sampleReportConfig }
			eventHandlers = { eventHandlersMap }
			cssClassName = { reportClass }
			getEmbeddedComponent = { (embedObject: Embed) => {
				console.log(`Embedded object of type "${ embedObject.embedtype }" received`);
				setReport(embedObject as Report);
			} }
		/>;

	return (
		<div className = "container">

			<div className = "controls">
				{ controlButtons }

				{ isEmbedded ? reportComponent : null }
			</div>

		</div>
	);
}

export default DemoApp;