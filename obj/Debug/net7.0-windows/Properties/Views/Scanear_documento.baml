   M S B A M L   `   `   `  ����  Portal.Kiosco%    Portal.Kiosco.Scanear_documento   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � WindowStyle$
  None=�5   	   
������� � WindowState$ 	Maximized=�5	   	    � Title$ Scanear_documento��5
   	   $
��1080��6#   $
��1920��61    � 	Resources 5         MyButton �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers	 P� Property
 P� Value��%�  &	        �� 5   
   " ��6!   .�5      ��  �@�OverridesDefaultStyle$
 True��66   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $��border��6!   $��0q�6/   
���   �#��z��6W   ._�5      �� $��Center=�6/   $��Center=�6L   5       5      P� 5      	 �@J�IsMouseOver$

 True��6=   . �5   "   ��  �@J�Opacity$	 0.8Q�6<   5      5      5      5      5      .��5       �� ._�5!   
   �  � RowDefinitions 5"      �� 5#      $��169.29 �6    �� 5$      $	��240 �6    �� 5%      $	��123 �6    �� 5&      .+�5*      �� $��0��6   $��212.55��6"   $��85.29��61   $��Bottom=�6@   $
��Fill=�6[   $R��LC:\FALCROSOFT\PROCINAL\Portal.Kiosco\Properties\Resources\logo-procinal1.png��6j   �� 5,      $��1��6   $
��1413��6%   $	��115��62   $��Bottom=�6?   $��48(�6Z   $��Myanmar Khyay)�6h    �� TextAlignment$ Center=�6�   *(Coloca tu cédula o tarjeta de identidad�� 5.      en el lector50      �� 51      -   5         ��Name$ TextDocumento��51      
������ 
������ $��20(�6`   $��4,4,1524,134q�6n   .��52   8   ��53      �� 54      " ��6   .�55      ��  � ��$ 1q�6<   �� 56       � ��
 ���� 68    �� Triggers 57      P� 58      	 �@J�	IsFocused$

 True��6;   . �59   "   ��  � ��$ 0q�6D   5:      5<      .��5=   !   5>      �� 5A      $��2��6   $	��562��6"   $��80��6.   ���#D9D9D9$��Bottom=�6O    �� CornerRadius$ 10��6j   ._�5B      �� -   5        $��btnIngresaDocumento��5B      
������ $��0q�6o   ���#F30613$��Myanmar Khyay)�6�   $��20(�6�   $	��562��6�   $��80��6�   #��[  6�   .��5C      �� $ Center=�6    +)No traje mi documento de identificación,�� 5E      comprar como invitado5G       J� Effect 5H      3  -System.Windows.Media.Effects.DropShadowEffect  5I         Color$
 Gray��6+      	Direction$	 270Q�68      ShadowDepth$ 3Q�6H      Opacity$	 0.5Q�6X   5K       5M        5N      $
 Gray��6'   $	 270Q�64   $ 2Q�6D   $	 0.5Q�6T   5Q      �� 5S      $��3��6   $	��330��6!   $	��330��6-   $	��Top=�6:   $��0,40,0,0q�6R   $
��Fill=�6d   $Q��KC:\FALCROSOFT\PROCINAL\Portal.Kiosco\Properties\Resources\ScannerCedula.png��6s   5V      5W      5X      