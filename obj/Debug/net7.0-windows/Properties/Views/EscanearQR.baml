   M S B A M L   `   `   `  ����H  DPortal.Kiosco, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Portal.KioscoPortal.Kiosco  5   	       Portal.Kiosco.EscanearQR   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:Portal.Kiosco     5   	     � Title$  
EscanearQR��5   	   $
��1080��6   $
��1920��6*    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�5
   	   
������  � AllowsTransparency$
 True��5   	   .��5      �� ._�5   
   � .+�5      �� 
���   � J� Opacity$	 0.7Q�6(   �� 5      $	��523��6   $	��502��6"    �� CornerRadius$ 15��6.   ���#2F2A29._�5      �  � RowDefinitions 5      �� 5      $��40 �6(   �� 5       � ColumnDefinitions 5      �� 5      $��50 �6+   �� 5      �� 5      $��50 �6+   .+�5      �� $��1��6   $��1��6*   ._�5      � .+�5       �� -   5         ��Name$ circle��5    $   $��/ellipse-209.png��64   $
��Fill=�6N   $��Center=�6]   $	��240��6x   $��Center=�6�   $	��230��6�   �� 5!      -   5        $ circle2��5!   $   $��/Ellipse 218.png��65   $
��Fill=�6O   $��Center=�6^   $	��294��6y   $��Center=�6�   $	��280��6�   �� 5"      -   5        $ circle3��5"   $   $��/Ellipse 217.png��65   $
��Fill=�6O   $��Center=�6^   $	��346��6y   $��Center=�6�   $	��328��6�   �� 5#      -   5        $ scanner��5#   $   $��/qrcodescanner.png��65   $
��Fill=�6Q   $��Center=�6`   $	��206��6{   $��Center=�6�   $	��162��6�   5$      5%      �� 5&      $��1��6    $��1��6-   $��Bienvenido de Nuevo��6=   
�������$��20(�6k   $��Poppins Semibold)�6y   $	��Top=�6�   �� 5'      $��1��6    $��1��6-   
�������$��16(�6P   $��Poppins Regular)�6^   $	��Top=�6{   $
��0,30q�6�   /-Coloca QR generado desde tu App en el scanner�� 5)      para iniciar sesión.5+      �� 5-      $��	close.png��6   $
��Fill=�6/   $��2��6>   $��Bottom=�6N   $��30��6i   $��30��6t   $
��left=�6�   	 � Cursor$
	 Hand��6�   5/      50      51      52      53      