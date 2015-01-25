'use strict';

angular.module('mainApp').controller('StrategyDetailsCtrl', [
	'$scope',
	'$routeParams',
	'$location',
	'ConfigFactory',
	'OrderListFactory',
	'StrategyListFactory',
	function(
		$scope,
		$routeParams,
		$location,
		ConfigFactory,
		OrderListFactory,
		StrategyListFactory
	) {
		// Save since it will be used in the rest of the app.
		ConfigFactory.setOutputFolder($routeParams.runName);

		$scope.strategy = $routeParams.strategy;
		$scope.ticker = $routeParams.ticker;

		$scope.orders = [];
		StrategyListFactory.getStrategy($scope.strategy, $scope.ticker).then(function(data) {
			$scope.orders = data.orders;

			$scope.chartEvents = OrderListFactory.convertOrdersToDataSeries(data.orders);

			// Add all the indicators to the chart.
			for (var i = 0; i < data.indicators.length; i++) {
				$scope.$broadcast('AddIndicator', { name: data.indicators[i] });
			}
		});


		/**
		 * Snap the chart to the order location.
		 * @param  {Object} order Order that was clicked
		 */
		$scope.orderClick = function(order) {
			console.log('Snap the chart to the order location');
		};

	} // end controller
]);

